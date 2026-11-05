using AutoMapper;
using BankCore.Application.DTOs.Responses;
using BankCore.Domain.Entities;

namespace BankCore.Application.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerResponse>();
    }
}
