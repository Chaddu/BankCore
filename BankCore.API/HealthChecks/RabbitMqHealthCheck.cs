using BankCore.Infrastructure.Messaging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BankCore.API.HealthChecks;

public class RabbitMqHealthCheck(RabbitMqConnection rabbitMqConnection) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext healthCheckContext, CancellationToken cancellationToken = default)
    {
        try
        {
            if(!rabbitMqConnection.Connection.IsOpen)
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is closed."));

            return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ connection is open."));
        }
        catch(Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ health check failed.", ex));
        }
    }
}
