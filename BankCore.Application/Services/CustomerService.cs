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

    public async Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var customer = await unitOfWork.Customers.GetByIdAsync(id);

        if (customer is null)
            return Result<CustomerResponse>.Failure("Customer not found", ErrorType.NotFound);

        var email = request.Email.Trim().ToLowerInvariant();

        var existingCustomer = await unitOfWork.Customers.GetByEmailAsync(email);

        if (existingCustomer is not null && existingCustomer.Id != id)
            return Result<CustomerResponse>.Failure("A customer with this email already exists.",ErrorType.Conflict);

        try
        {
            customer.UpdateDetails(
                request.FirstName,
                request.LastName,
                email);

            await unitOfWork.SaveChangesAsync();

            var response = mapper.Map<CustomerResponse>(customer);
            return Result<CustomerResponse>.Success(response);
        }
        catch (DomainException ex)
        {
            return Result<CustomerResponse>.Failure(ex.Message,ErrorType.Validation);
        }
    }
}
