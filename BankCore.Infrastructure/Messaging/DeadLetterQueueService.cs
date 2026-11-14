using BankCore.Application.Common;
using BankCore.Application.Constants;
using BankCore.Application.Interfaces.Services;
using RabbitMQ.Client;

namespace BankCore.Infrastructure.Messaging;

public class DeadLetterQueueService(
    RabbitMqConnection rabbitMqConnection) : IDeadLetterQueueService
{
    public async Task<Result> ReplayNextMessageAsync(
       CancellationToken cancellationToken = default)
    {
        await using var channel =
            await rabbitMqConnection.Connection.CreateChannelAsync(
                new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true),
                cancellationToken);

        var message = await channel.BasicGetAsync(
            RabbitMqConstants.NotificationDeadLetterQueue,
            autoAck: false,
            cancellationToken);

        if (message is null)
            return Result.Failure(
                "Dead-letter queue is empty.",
                ErrorType.NotFound);

        try
        {
            var originalProperties = message.BasicProperties;

            var properties = new BasicProperties
            {
                ContentType = originalProperties.ContentType,
                ContentEncoding = originalProperties.ContentEncoding,
                MessageId = originalProperties.MessageId,
                CorrelationId = originalProperties.CorrelationId,
                Type = originalProperties.Type,
                DeliveryMode = DeliveryModes.Persistent,
                Headers = originalProperties.Headers
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: RabbitMqConstants.NotificationQueue,
                mandatory: true,
                basicProperties: properties,
                body: message.Body,
                cancellationToken);

            await channel.BasicAckAsync(
                message.DeliveryTag,
                multiple: false,
                cancellationToken);

            return Result.Success();
        }
        catch
        {
            await channel.BasicNackAsync(
                message.DeliveryTag,
                multiple: false,
                requeue: true,
                cancellationToken);

            throw;
        }
    }
}