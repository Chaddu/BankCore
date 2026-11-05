using BankCore.Application.Common;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;
using BankCore.Application.Profiles;
using BankCore.Application.Services;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using BankCore.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddAutoMapper(cfg => { }, typeof(CustomerProfile));

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBankAccountService, BankAccountService>();
        services.AddScoped<ILedgerEntryService, LedgerEntryService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
