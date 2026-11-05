using AutoMapper;
using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using BankCore.Domain.Enums;
using BankCore.Domain.Exceptions;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace BankCore.Application.Services;

public class TransferService(IUnitOfWork unitOfWork, IMapper mapper) : ITransferService
{
    public async Task<Result<TransferResponse>> CreateAsync(CreateTransferRequest request, string idempotencyKey)
    {
        if(string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return Result<TransferResponse>.Failure("Idempotency key is required.", ErrorType.Validation);
        }

        var requestHash = GenerateRequestHash(request);

        var existingRecord = await unitOfWork.IdempotencyRecords.GetByKeyAsync(idempotencyKey);

        if(existingRecord is not null)
        {
            if(existingRecord.RequestHash != requestHash)
            {
                return Result<TransferResponse>.Failure("This idempotency key was already used for a different request.", ErrorType.Conflict);
            }

            var existingTransfer = await unitOfWork.Transfers.GetByTransactionIdAsync(existingRecord.TransactionId);

            if (existingTransfer is null)
                return Result<TransferResponse>.Failure("The previous transfer record could not be found.",ErrorType.Failure);
           
            return Result<TransferResponse>.Success(mapper.Map<TransferResponse>(existingTransfer));
        }

        var fromAccount = await unitOfWork.BankAccounts.GetByIdAsync(request.FromAccountId);

        if(fromAccount is null)
            return Result<TransferResponse>.Failure("Source account not found.", ErrorType.NotFound);

        var toAccount = await unitOfWork.BankAccounts.GetByIdAsync(request.ToAccountId);

        if(toAccount is null)
            return Result<TransferResponse>.Failure("Destination account not found.", ErrorType.NotFound);

        try
        {
            var currency = request.Currency?.Trim().ToUpperInvariant();

            if(fromAccount.Currency != toAccount.Currency || fromAccount.Currency != currency)
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
                idempotencyKey.Trim(),
                requestHash,
                transactionId,
                DateTime.UtcNow.AddHours(24));

            await unitOfWork.Transfers.AddAsync(transfer);
            await unitOfWork.LedgerEntries.AddAsync(outgoingEntry);
            await unitOfWork.LedgerEntries.AddAsync(incomingEntry);
            await unitOfWork.IdempotencyRecords.AddAsync(idempotencyRecord);

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();

            return Result<TransferResponse>.Success(mapper.Map<TransferResponse>(transfer));
        }
        catch(DomainException ex)
        {
            await unitOfWork.RollbackTransactionAsync();

            return Result<TransferResponse>.Failure(ex.Message,ErrorType.Validation);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Result<TransferResponse>> GetByIdAsync(int id)
    {
        var transfer = await unitOfWork.Transfers.GetByIdAsync(id);

        if (transfer is null)
        {
            return Result<TransferResponse>.Failure("Transfer not found.",ErrorType.NotFound);
        }

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
