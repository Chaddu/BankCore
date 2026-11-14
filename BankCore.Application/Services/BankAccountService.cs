using AutoMapper;
using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using BankCore.Domain.Enums;
using BankCore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Application.Services;

public class BankAccountService(IUnitOfWork unitOfWork, IMapper mapper) : IBankAccountService
{
    public async Task<Result<AccountResponse>> CreateAsync(CreateAccountRequest request, int currentUserId)
    {
        var owner = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (owner is null)
            return Result<AccountResponse>.Failure("Customer not found.", ErrorType.NotFound);

        try
        {
            string accountNumber;

            do
            {
                accountNumber = AccountNumberGenerator.Generate();
            }
            while (await unitOfWork.BankAccounts.ExistsByAccountNumberAsync(accountNumber));

            var account = new BankAccount(owner.Id,accountNumber,request.Currency);

            await unitOfWork.BankAccounts.AddAsync(account);
            await unitOfWork.SaveChangesAsync();

            return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
        }
        catch (DomainException ex)
        {
            return Result<AccountResponse>.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<Result<AccountResponse>> GetByAccountNumberAsync(string accountNumber, int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<AccountResponse>.Failure("Customer not found.", ErrorType.NotFound);


        var account = await unitOfWork.BankAccounts.GetByAccountNumberAsync(accountNumber);

        if (account is null || account.OwnerId != customer.Id)
            return Result<AccountResponse>.Failure("Bank account not found.", ErrorType.NotFound);

        return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
    }

    public async Task<Result<AccountResponse>> GetByIdAsync(int id, int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<AccountResponse>.Failure("Customer not found.", ErrorType.NotFound);

        var account = await unitOfWork.BankAccounts.GetByIdAsync(id);

        if (account is null || account.OwnerId != customer.Id)
            return Result<AccountResponse>.Failure("Bank account not found.", ErrorType.NotFound);

        return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
    }

    public async Task<Result<List<AccountResponse>>> GetByOwnerIdAsync(int ownerId)
    {
        var owner = await unitOfWork.Customers.GetByIdAsync(ownerId);

        if (owner is null)
            return Result<List<AccountResponse>>.Failure("Customer not found.", ErrorType.NotFound);

        var accounts = await unitOfWork.BankAccounts.GetByOwnerIdAsync(ownerId);

        return Result<List<AccountResponse>>.Success(mapper.Map<List<AccountResponse>>(accounts));
    }
    public async Task<Result<List<AccountResponse>>> GetMyAccountsAsync(int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);
        if (customer is null)
            return Result<List<AccountResponse>>.Failure("Customer not found.", ErrorType.NotFound);

        var accounts = await unitOfWork.BankAccounts.GetByOwnerIdAsync(customer.Id);

        return Result<List<AccountResponse>>.Success(mapper.Map<List<AccountResponse>>(accounts));
    }
    public async Task<Result<AccountResponse>> DepositAsync(int accountId, DepositRequest request, int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<AccountResponse>.Failure("Customer not found.", ErrorType.NotFound);

        var account = await unitOfWork.BankAccounts.GetByIdAsync(accountId);

        if (account is null || account.OwnerId != customer.Id)
            return Result<AccountResponse>.Failure("Account not found.", ErrorType.NotFound);

        try
        {
            account.Deposit(request.Amount);

            var ledgerEntry = new LedgerEntry(
                account.Id,
                request.Amount,
                TransactionType.Deposit,
                "Account deposit",
                Guid.NewGuid());

            await unitOfWork.LedgerEntries.AddAsync(ledgerEntry);
            await unitOfWork.SaveChangesAsync();

            return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<AccountResponse>.Failure(
                "The account balance was modified by another transaction. Please try again.",
                ErrorType.Conflict);
        }
        catch (DomainException ex)
        {
            return Result<AccountResponse>.Failure(ex.Message, ErrorType.Validation);
        }
    }
}
