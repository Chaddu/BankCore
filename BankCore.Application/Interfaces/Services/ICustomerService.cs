using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;
public interface ICustomerService
{
    Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request);

    Task<Result<CustomerResponse?>> GetByIdAsync(int id);

    Task<Result<CustomerResponse?>> GetByEmailAsync(string email);

    Task<Result<CustomerResponse?>> UpdateAsync(int id,UpdateCustomerRequest request);
}
