namespace BankCore.Application.DTOs.Responses;

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt);