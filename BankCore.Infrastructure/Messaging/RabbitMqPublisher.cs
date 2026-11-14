using BankCore.Application.Interfaces.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BankCore.Infrastructure.Messaging;

public class RabbitMqPublisher(RabbitMqConnection rabbitMqConnection) : IMessagePublisher
{
    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default)
    {
        await using var channel = await rabbitMqConnection.Connection.CreateChannelAsync(
    new CreateChannelOptions(
        publisherConfirmationsEnabled: true,
        publisherConfirmationTrackingEnabled: true));

        //await channel.QueueDeclareAsync(
        //    queue: routingKey,
        //    durable: true,
        //    exclusive: false,
        //    autoDelete: false,
        //    cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}
