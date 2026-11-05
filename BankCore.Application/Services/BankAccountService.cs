using AutoMapper;
using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using BankCore.Domain.Exceptions;

namespace BankCore.Application.Services;

public class BankAccountService(IUnitOfWork unitOfWork, IMapper mapper) : IBankAccountService
{
    public async Task<Result<AccountResponse>> CreateAsync(CreateAccountRequest request)
    {
        var owner = await unitOfWork.Customers.GetByIdAsync(request.OwnerId);

        if (owner is null)
            return Result<AccountResponse>.Failure("Customer not found.",ErrorType.NotFound);

        try
        {
            var accountNumber = AccountNumberGenerator.Generate();

            var account = new BankAccount(
                request.OwnerId,
                accountNumber,
                request.Currency);

            await unitOfWork.BankAccounts.AddAsync(account);
            await unitOfWork.SaveChangesAsync();

            return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
        }
        catch (DomainException ex)
        {
            return Result<AccountResponse>.Failure(ex.Message,ErrorType.Validation);
        }
    }

    public async Task<Result<AccountResponse>> GetByAccountNumberAsync(string accountNumber)
    {
        var account= await unitOfWork.BankAccounts.GetByAccountNumberAsync(accountNumber);

        if(account is null)
            return Result<AccountResponse>.Failure("Bank account not found.", ErrorType.NotFound);
        
        return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
    }

    public async Task<Result<AccountResponse>> GetByIdAsync(int id)
    {
        var account = await unitOfWork.BankAccounts.GetByIdAsync(id);

        if (account is null)
            return Result<AccountResponse>.Failure("Bank account not found.", ErrorType.NotFound);

        return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
    }

    public async Task<Result<List<AccountResponse>>> GetByOwnerIdAsync(int ownerId)
    {
        var owner = await unitOfWork.Customers.GetByIdAsync(ownerId);

        if(owner is null)
            return Result<List<AccountResponse>>.Failure("Customer not found.", ErrorType.NotFound);

        var accounts = await unitOfWork.BankAccounts.GetByOwnerIdAsync(ownerId);

        return Result<List<AccountResponse>>.Success(accounts.Select(mapper.Map<AccountResponse>).ToList());
    }
}
