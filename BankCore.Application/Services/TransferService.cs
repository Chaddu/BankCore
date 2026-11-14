using AutoMapper;
using BankCore.Application.Common;
using BankCore.Application.Constants;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Events;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Messaging;
using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using BankCore.Domain.Enums;
using BankCore.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BankCore.Application.Services;

public class TransferService(IUnitOfWork unitOfWork, IMapper mapper) : ITransferService
{
    public async Task<Result<TransferResponse>> CreateAsync(CreateTransferRequest request, string idempotencyKey, int currentUserId)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return Result<TransferResponse>.Failure("Idempotency key is required.", ErrorType.Validation);

        idempotencyKey = idempotencyKey.Trim();

        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<TransferResponse>.Failure("Customer not found.", ErrorType.NotFound);

        var requestHash = GenerateRequestHash(request);

        var existingRecord = await unitOfWork.IdempotencyRecords.GetByKeyAsync(idempotencyKey);

        if (existingRecord is not null && existingRecord.ExpiresAt <= DateTime.UtcNow)
        {
            await unitOfWork.IdempotencyRecords.DeleteAsync(existingRecord);
            await unitOfWork.SaveChangesAsync();

            existingRecord = null;
        }

        if (existingRecord is not null)
        {
            if (existingRecord.RequestHash != requestHash)
            {
                return Result<TransferResponse>.Failure("This idempotency key was already used for a different request.", ErrorType.Conflict);
            }

            var existingTransfer = await unitOfWork.Transfers.GetByTransactionIdAsync(existingRecord.TransactionId);

            if (existingTransfer is null)
                return Result<TransferResponse>.Failure("The previous transfer record could not be found.", ErrorType.Failure);

            var existingSourceAccount = await unitOfWork.BankAccounts.GetByIdAsync(existingTransfer.FromAccountId);

            if (existingSourceAccount is null || existingSourceAccount.OwnerId != customer.Id)
                return Result<TransferResponse>.Failure("Transfer not found.", ErrorType.NotFound);

            return Result<TransferResponse>.Success(mapper.Map<TransferResponse>(existingTransfer));
        }

        var fromAccount = await unitOfWork.BankAccounts.GetByIdAsync(request.FromAccountId);

        if (fromAccount is null)
            return Result<TransferResponse>.Failure("Source account not found.", ErrorType.NotFound);

        if (fromAccount.OwnerId != customer.Id)
            return Result<TransferResponse>.Failure(
                "You are not allowed to transfer from this account.",
                ErrorType.Forbidden);

        var toAccount = await unitOfWork.BankAccounts.GetByIdAsync(request.ToAccountId);

        if (toAccount is null)
            return Result<TransferResponse>.Failure("Destination account not found.", ErrorType.NotFound);

        try
        {
            var currency = request.Currency?.Trim().ToUpperInvariant();

            if (fromAccount.Currency != toAccount.Currency || fromAccount.Currency != currency)
                return Result<TransferResponse>.Failure("Both accounts and the request must use the same currency.", ErrorType.Validation);

            var transfer = new Transfer(
                request.FromAccountId,
                request.ToAccountId,
                request.Amount,
                currency,
                request.Description);

            var transactionId = transfer.TransactionId;

            await unitOfWork.BeginTransactionAsync();

            fromAccount.Withdraw(request.Amount);
            toAccount.Deposit(request.Amount);

            var outgoingEntry = new LedgerEntry(
                fromAccount.Id,
                request.Amount,
                TransactionType.TransferOut,
                $"Transfer to account {toAccount.AccountNumber}",
                transactionId);

            var incomingEntry = new LedgerEntry(
                toAccount.Id,
                request.Amount,
                TransactionType.TransferIn,
                $"Transfer from account {fromAccount.AccountNumber}",
                transactionId);

            transfer.Complete();

            var idempotencyRecord = new IdempotencyRecord(
                idempotencyKey,
                requestHash,
                transactionId,
                DateTime.UtcNow.AddHours(24));

            await unitOfWork.Transfers.AddAsync(transfer);
            await unitOfWork.LedgerEntries.AddAsync(outgoingEntry);
            await unitOfWork.LedgerEntries.AddAsync(incomingEntry);
            await unitOfWork.IdempotencyRecords.AddAsync(idempotencyRecord);

            var transferEvent = new TransferCompletedEvent(
                transfer.TransactionId,
                fromAccount.OwnerId,
                toAccount.OwnerId,
                transfer.Amount,
                transfer.Currency,
                DateTime.UtcNow);

            var outboxMessage = new OutboxMessage(nameof(TransferCompletedEvent),JsonSerializer.Serialize(transferEvent));

            await unitOfWork.OutboxMessages.AddAsync(outboxMessage);

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();

            return Result<TransferResponse>.Success(mapper.Map<TransferResponse>(transfer));
        }
        catch (DbUpdateConcurrencyException)
        {
            await unitOfWork.RollbackTransactionAsync();

            return Result<TransferResponse>.Failure("The account balance was modified by another transaction. Please try again.", ErrorType.Conflict);
        }
        catch (DomainException ex)
        {
            await unitOfWork.RollbackTransactionAsync();

            return Result<TransferResponse>.Failure(ex.Message, ErrorType.Validation);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2601 || sqlException.Number == 2627))
        {
            await unitOfWork.RollbackTransactionAsync();

            return Result<TransferResponse>.Failure("This transfer request has already been processed. Please retry using the same idempotency key.", ErrorType.Conflict);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Result<TransferResponse>> GetByIdAsync(int id, int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<TransferResponse>.Failure("Customer not found.", ErrorType.NotFound);

        var transfer = await unitOfWork.Transfers.GetByIdAsync(id);

        if (transfer is null)
        {
            return Result<TransferResponse>.Failure("Transfer not found.", ErrorType.NotFound);
        }

        var fromAccount = await unitOfWork.BankAccounts.GetByIdAsync(transfer.FromAccountId);

        var toAccount = await unitOfWork.BankAccounts.GetByIdAsync(transfer.ToAccountId);

        if (fromAccount is null || toAccount is null)
            return Result<TransferResponse>.Failure("Transfer not found.", ErrorType.NotFound);

        var isSender = fromAccount.OwnerId == customer.Id;
        var isRecipient = toAccount.OwnerId == customer.Id;

        if (!isSender && !isRecipient)
            return Result<TransferResponse>.Failure("Transfer not found.", ErrorType.NotFound);

        return Result<TransferResponse>.Success(mapper.Map<TransferResponse>(transfer));

    }

    private static string GenerateRequestHash(CreateTransferRequest request)
    {
        var canonicalRequest = string.Join(
            "|",
            request.FromAccountId.ToString(CultureInfo.InvariantCulture),
            request.ToAccountId.ToString(CultureInfo.InvariantCulture),
            request.Amount.ToString(CultureInfo.InvariantCulture),
            request.Currency?.Trim().ToUpperInvariant() ?? string.Empty,
            request.Description?.Trim() ?? string.Empty);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonicalRequest));

        return Convert.ToHexString(bytes);
    }
}
