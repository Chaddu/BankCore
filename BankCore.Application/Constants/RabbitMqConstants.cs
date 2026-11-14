namespace BankCore.Application.Constants;

public class RabbitMqConstants
{
    public const string NotificationQueue = "bankcore.notifications";
    public const string NotificationDeadLetterExchange = "bankcore.notifications.dlx";
    public const string NotificationDeadLetterQueue = "bankcore.notifications.dead";
}
