using BankCore.Application.Constants;
using BankCore.Application.Events;
using BankCore.Application.Interfaces;
using BankCore.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Messaging.Consumers;

public class NotificationConsumer(RabbitMqConnection rabbitMqConnection, ILogger<NotificationConsumer> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await rabbitMqConnection.Connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.NotificationQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = RabbitMqConstants.NotificationDeadLetterExchange,
                ["x-dead-letter-routing-key"] = RabbitMqConstants.NotificationDeadLetterQueue
            },
    cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.NotificationDeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.NotificationDeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.NotificationDeadLetterQueue,
            exchange: RabbitMqConstants.NotificationDeadLetterExchange,
            routingKey: RabbitMqConstants.NotificationDeadLetterQueue,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var transferEvent =
                    JsonSerializer.Deserialize<TransferCompletedEvent>(message);

                if (transferEvent is null)
                    throw new InvalidOperationException("Invalid transfer event.");

                await using var scope = scopeFactory.CreateAsyncScope();

                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                logger.LogInformation("Received transfer event {TransactionId}", transferEvent.TransactionId);

                var senderExists = await unitOfWork.Notifications
                    .ExistsByTransactionIdAsync(
                    transferEvent.TransactionId,
                    transferEvent.FromCustomerId);

                var recipientExists = await unitOfWork.Notifications
                    .ExistsByTransactionIdAsync(
                        transferEvent.TransactionId,
                        transferEvent.ToCustomerId);

                if (!senderExists)
                {
                    var senderNotification = new Notification(
                        transferEvent.FromCustomerId,
                        transferEvent.TransactionId,
                        "Transfer completed",
                        $"Your transfer of {transferEvent.Amount} {transferEvent.Currency} has been sent successfully.");

                    await unitOfWork.Notifications.AddAsync(senderNotification);
                }

                if (!recipientExists)
                {
                    var recipientNotification = new Notification(
                        transferEvent.ToCustomerId,
                        transferEvent.TransactionId,
                        "Money received",
                        $"You have received {transferEvent.Amount} {transferEvent.Currency}.");

                    await unitOfWork.Notifications.AddAsync(recipientNotification);
                }

                await unitOfWork.SaveChangesAsync();

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
            {
                logger.LogWarning(
                    "Duplicate notification detected. Acknowledging message.");

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process notification message.");

                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false);

        await channel.BasicConsumeAsync(
            queue: RabbitMqConstants.NotificationQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private static bool IsDuplicateKeyException(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException
            && (sqlException.Number == 2601 || sqlException.Number == 2627);
    }
}

