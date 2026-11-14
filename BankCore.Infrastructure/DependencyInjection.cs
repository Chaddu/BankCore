using BankCore.Application.Common;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Messaging;
using BankCore.Application.Interfaces.Services;
using BankCore.Application.Profiles;
using BankCore.Application.Services;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Messaging;
using BankCore.Infrastructure.Messaging.Consumers;
using BankCore.Infrastructure.Persistence;
using BankCore.Infrastructure.Security;
using BankCore.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

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

        services.Configure<RabbitMqSettings>(
    configuration.GetSection("RabbitMq"));
        services.AddSingleton<RabbitMqConnection>();

        services.AddSingleton<IHostedService>(serviceProvider =>
            serviceProvider.GetRequiredService<RabbitMqConnection>());


        services.AddHostedService<NotificationConsumer>();
        services.AddHostedService<OutboxProcessor>();

        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBankAccountService, BankAccountService>();
        services.AddScoped<ILedgerEntryService, LedgerEntryService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDeadLetterQueueService, DeadLetterQueueService>();


        return services;
    }
}
