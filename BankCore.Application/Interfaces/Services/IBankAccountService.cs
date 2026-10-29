using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;
public interface IBankAccountService
{
    Task<Result<AccountResponse>> CreateAsync(CreateAccountRequest request);

    Task<Result<AccountResponse?>> GetByIdAsync(int id);

    Task<Result<AccountResponse?>> GetByAccountNumberAsync(string accountNumber);

    Task<Result<List<AccountResponse>>> GetByOwnerIdAsync(int ownerId);
}
