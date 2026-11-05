using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);

    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
}
