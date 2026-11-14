using BankCore.Infrastructure.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BankCore.Infrastructure.Messaging;

public class RabbitMqConnection(IOptions<RabbitMqSettings> options) : IHostedService, IAsyncDisposable
{
    private IConnection? connection;

    public IConnection Connection => connection ?? throw new InvalidOperationException("RabbitMQ connection has not been initialized.");

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            ClientProvidedName = "BankCore.API"
        };

        const int maxAttempts = 5;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                connection = await factory.CreateConnectionAsync(cancellationToken);
                return;
            }
            catch (Exception) when (attempt < maxAttempts)
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);
            }
        }

        throw new InvalidOperationException(
            "Could not establish a connection to RabbitMQ after multiple attempts.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (connection is not null)
            await connection.CloseAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (connection is not null)
            await connection.DisposeAsync();
    }
}
