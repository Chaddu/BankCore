using AutoMapper;
using BankCore.Application.DTOs.Responses;
using BankCore.Domain.Entities;

namespace BankCore.Application.Profiles;
public class BankAccountProfile : Profile
{
    public BankAccountProfile()
    {
        CreateMap<BankAccount, AccountResponse>();
    }
}
