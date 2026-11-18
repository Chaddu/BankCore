using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using BankCore.Domain.Exceptions;
using System.Net.Mail;

namespace BankCore.Application.Services;

public class AuthService(IUnitOfWork unitOfWork, IPasswordService passwordService, IJwtService jwtService) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        var user = await unitOfWork.Users.GetByEmailAsync(email);

        var isPasswordValid = passwordService.VerifyPassword(user, password);

        if (user is null || !isPasswordValid)
            return Result<AuthResponse>.Failure("Invalid email or password.",ErrorType.Unauthorized);

        var (token, expiresAt) = jwtService.GenerateToken(
            user.Id,
            user.Email,
            user.Role.ToString());

        return Result<AuthResponse>.Success(new AuthResponse(token, expiresAt));
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var firstName = request.FirstName?.Trim() ?? string.Empty;
        var lastName = request.LastName?.Trim() ?? string.Empty;
        var email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName))
            return Result<AuthResponse>.Failure("First name is required.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<AuthResponse>.Failure("Last name is required.", ErrorType.Validation);

        if (!MailAddress.TryCreate(email, out _))
            return Result<AuthResponse>.Failure("Invalid email address.", ErrorType.Validation);

        if (password.Length < 8)
            return Result<AuthResponse>.Failure("Password must contain at least 8 characters.", ErrorType.Validation);

        if (!password.Any(char.IsUpper))
            return Result<AuthResponse>.Failure("Password must contain at least one uppercase letter.", ErrorType.Validation);

        if (!password.Any(char.IsLower))
            return Result<AuthResponse>.Failure("Password must contain at least one lowercase letter.", ErrorType.Validation);

        if (!password.Any(char.IsDigit))
            return Result<AuthResponse>.Failure("Password must contain at least one digit.", ErrorType.Validation);

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            return Result<AuthResponse>.Failure("Password must contain at least one special character.", ErrorType.Validation);

        if (request.DateOfBirth.Date >= DateTime.UtcNow.Date)
            return Result<AuthResponse>.Failure("Date of birth must be in the past.", ErrorType.Validation);

        if (await unitOfWork.Users.ExistsByEmailAsync(email))
            return Result<AuthResponse>.Failure("A user with this email already exists.", ErrorType.Conflict);

        try
        {
            await unitOfWork.BeginTransactionAsync();

            var user = new User(email);

            var passwordHash = passwordService.HashPassword(user, password);
            user.SetPasswordHash(passwordHash);

            await unitOfWork.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            var customer = new Customer(
                user.Id,
                firstName,
                lastName,
                request.DateOfBirth);

            await unitOfWork.Customers.AddAsync(customer);
            await unitOfWork.SaveChangesAsync();

            await unitOfWork.CommitTransactionAsync();

            var (token, expiresAt) = jwtService.GenerateToken(
                user.Id,
                user.Email,
                user.Role.ToString());

            return Result<AuthResponse>.Success(new AuthResponse(token, expiresAt));
        }
        catch (DomainException ex)
        {
            await unitOfWork.RollbackTransactionAsync();

            return Result<AuthResponse>.Failure(ex.Message,ErrorType.Validation);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
