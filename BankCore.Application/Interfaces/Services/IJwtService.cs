namespace BankCore.Application.Interfaces.Services;

public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateToken(int userId,string email,string role);
}

