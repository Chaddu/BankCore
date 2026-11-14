using AutoMapper;
using BankCore.Application.DTOs.Responses;
using BankCore.Domain.Entities;

namespace BankCore.Application.Profiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationResponse>();
    }
}
