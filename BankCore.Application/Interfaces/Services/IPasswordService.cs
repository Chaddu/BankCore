using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Services;

public interface IPasswordService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string password);
}
