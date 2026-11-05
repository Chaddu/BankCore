using AutoMapper;
using BankCore.Application.DTOs.Responses;
using BankCore.Domain.Entities;

namespace BankCore.Application.Profiles;
public class TransferProfile : Profile
{
    public TransferProfile()
    {
        CreateMap<Transfer, TransferResponse>();
    }
}
