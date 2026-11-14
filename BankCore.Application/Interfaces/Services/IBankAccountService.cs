using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;
public interface IBankAccountService
{
    Task<Result<AccountResponse>> CreateAsync(CreateAccountRequest request, int currentUserId);

    Task<Result<AccountResponse>> GetByIdAsync(int id, int currentUserId);

    Task<Result<AccountResponse>> GetByAccountNumberAsync(string accountNumber, int currentUserId);

    Task<Result<List<AccountResponse>>> GetByOwnerIdAsync(int ownerId);

    Task<Result<List<AccountResponse>>> GetMyAccountsAsync(int currentUserId);

    Task<Result<AccountResponse>> DepositAsync(int accountId,DepositRequest request,int currentUserId);

}
