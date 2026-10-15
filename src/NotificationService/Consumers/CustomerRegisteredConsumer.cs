using MassTransit;
using Shared.Contracts.Events;

namespace NotificationService.Consumers;

public class CustomerRegisteredConsumer(
    ProcessedMessageStore store,
    ILogger<CustomerRegisteredConsumer> logger) : IConsumer<CustomerRegistered>
{
    public async Task Consume(ConsumeContext<CustomerRegistered> context)
    {
        var messageId = context.MessageId ?? Guid.Empty;
        
        if (store.Contains(messageId))
        {
            logger.LogInformation("Дублікат {MessageId} пропущено", messageId);
            return;
        }

        var msg = context.Message;
        await Task.Delay(300);
        logger.LogInformation("Вітальний лист надіслано: {Email} ({Name}, {City})",
            msg.Email, msg.FullName, msg.City);

        store.Add(messageId);
        logger.LogInformation("Оброблено {MessageId}", messageId);
    }
}