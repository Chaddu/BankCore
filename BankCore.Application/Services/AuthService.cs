using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using BankCore.Domain.Exceptions;

namespace BankCore.Application.Services;

public class AuthService(IUnitOfWork unitOfWork, IPasswordService passwordService, IJwtService jwtService) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await unitOfWork.Users.GetByEmailAsync(email);

        if(user is null)
            return Result<AuthResponse>.Failure("Invalid email or password.",ErrorType.Unauthorized);

        var isPasswordValid = passwordService.VerifyPassword(user, request.Password);

        if(!isPasswordValid)
            return Result<AuthResponse>.Failure("Invalid email or password.", ErrorType.Unauthorized);

        var (token, expiresAt) = jwtService.GenerateToken(
       user.Id,
       user.Email,
       user.Role.ToString());

        return Result<AuthResponse>.Success(new AuthResponse(token, expiresAt));
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if(await unitOfWork.Users.ExistsByEmailAsync(email))
            return Result<AuthResponse>.Failure("A user with this email already exists.", ErrorType.Conflict);

        try
        {
            await unitOfWork.BeginTransactionAsync();

            var user = new User(email);

            var passwordHash = passwordService.HashPassword(user, request.Password);


            user.SetPasswordHash(passwordHash);


            await unitOfWork.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            var customer = new Customer(
            user.Id,
            request.FirstName,
            request.LastName,
            email,
            request.DateOfBirth);

            await unitOfWork.Customers.AddAsync(customer);
            await unitOfWork.SaveChangesAsync();

            await unitOfWork.CommitTransactionAsync();

            var (token, expiresAt) = jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());

            return Result<AuthResponse>.Success(new AuthResponse(token, expiresAt));
        }
        catch(DomainException ex)
        {
            await unitOfWork.RollbackTransactionAsync();

            return Result<AuthResponse>.Failure(
                ex.Message,
                ErrorType.Validation);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
