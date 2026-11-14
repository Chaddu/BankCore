using AutoMapper;
using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using BankCore.Domain.Exceptions;

namespace BankCore.Application.Services;

public class CustomerService(IUnitOfWork unitOfWork, IMapper mapper) : ICustomerService
{
    public async Task<Result<CustomerResponse>> GetByEmailAsync(string email)
    {
        var customer = await unitOfWork.Customers.GetByEmailAsync(email);

        if (customer is null)
            return Result<CustomerResponse>.Failure("Customer not found", ErrorType.NotFound);

        var response = mapper.Map<CustomerResponse>(customer);

        return Result<CustomerResponse>.Success(response);
    }

    public async Task<Result<CustomerResponse>> GetByIdAsync(int id)
    {
        var customer = await unitOfWork.Customers.GetByIdAsync(id);

        if (customer is null)
            return Result<CustomerResponse>.Failure("Customer not found", ErrorType.NotFound);

        var response = mapper.Map<CustomerResponse>(customer);

        return Result<CustomerResponse>.Success(response);
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(int currentUserId, UpdateCustomerRequest request)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<CustomerResponse>.Failure("Customer not found", ErrorType.NotFound);

        try
        {
            customer.UpdateDetails(
                request.FirstName,
                request.LastName);

            await unitOfWork.SaveChangesAsync();

            var response = mapper.Map<CustomerResponse>(customer);
            return Result<CustomerResponse>.Success(response);
        }
        catch (DomainException ex)
        {
            return Result<CustomerResponse>.Failure(ex.Message,ErrorType.Validation);
        }
    }
    public async Task<Result<CustomerResponse>> GetMyProfileAsync(int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<CustomerResponse>.Failure("Customer not found.",ErrorType.NotFound);

        return Result<CustomerResponse>.Success(mapper.Map<CustomerResponse>(customer));
    }
}
