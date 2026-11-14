using BankCore.Application.Constants;
using BankCore.Application.Events;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BankCore.Infrastructure.Messaging;

public class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    IMessagePublisher messagePublisher,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private const int MaxRetryCount = 5;
    private DateTime _lastCleanup = DateTime.MinValue;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
                if (DateTime.UtcNow - _lastCleanup >= TimeSpan.FromDays(1))
                {
                    await CleanupProcessedMessagesAsync(stoppingToken);
                    _lastCleanup = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing outbox messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var claimId = Guid.NewGuid();

        var messages = await unitOfWork.OutboxMessages.GetPendingMessagesAsync(
            batchSize: 20,
            claimId: claimId,
            claimDuration: TimeSpan.FromMinutes(2));

        foreach (var message in messages)
        {
            try
            {
                if (message.Type == nameof(TransferCompletedEvent))
                {
                    var transferEvent =
                        JsonSerializer.Deserialize<TransferCompletedEvent>(message.Content);

                    if (transferEvent is null)
                        throw new InvalidOperationException("Invalid outbox message.");

                    await messagePublisher.PublishAsync(
                        transferEvent,
                        RabbitMqConstants.NotificationQueue,
                        cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Unsupported outbox message type: {message.Type}");
                }

                var completed = await unitOfWork.OutboxMessages
                    .MarkAsProcessedAsync(message.Id, claimId);

                if (!completed)
                {
                    logger.LogWarning(
                        "Outbox message {EventId} was published, but could not be marked as processed because its claim expired or was taken over.",
                        message.EventId);
                }
            }
            catch (Exception ex)
            {
                var recorded = await unitOfWork.OutboxMessages.RecordFailureAsync(
                    message.Id,
                    claimId,
                    ex.Message,
                    MaxRetryCount);

                if (!recorded)
                {
                    logger.LogWarning(
                        ex,
                        "Could not record failure for outbox message {EventId} because its claim expired or was taken over.",
                        message.EventId);
                }
                else
                {
                    logger.LogError(
                        ex,
                        "Failed to publish outbox message {EventId}.",
                        message.EventId);
                }
            }
        }
    }

    private async Task CleanupProcessedMessagesAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();


        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var cutOffDate = DateTime.UtcNow.AddDays(-7);

        var deletedCount = await unitOfWork.OutboxMessages.DeleteProcessedMessagesAsync(cutOffDate);

        if (deletedCount > 0)
        {
            logger.LogInformation("Cleaned up {DeletedCount} processed outbox messages.", deletedCount);
        }
    }
}
