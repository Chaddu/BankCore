using BankCore.Application.DTOs.Requests;
using BankCore.Application.Interfaces.Services;
using BankCore.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BankCore.API.Controllers;

[EnableRateLimiting("AuthPolicy")]
[ApiController]
public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        return HandleResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        return HandleResult(result);
    }
}
