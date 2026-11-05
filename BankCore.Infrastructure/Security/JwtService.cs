using BankCore.Application.Common;
using BankCore.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankCore.Infrastructure.Security;

public class JwtService(IOptions<JwtSettings> options) : IJwtService
{
    private readonly JwtSettings settings = options.Value;

    public (string Token, DateTime ExpiresAt) GenerateToken(int userId, string email, string role)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(settings.ExpirationInMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Convert.FromBase64String(settings.SecretKey));

        var credentials = new SigningCredentials(
           key,
           SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenString, expiresAt);
    }
}
