using MassTransit;
using Shared.Contracts.Sagas;

namespace NotificationService.Consumers;

public class SendWelcomeConsumer(ILogger<SendWelcomeConsumer> logger) : IConsumer<SendWelcome>
{
    public async Task Consume(ConsumeContext<SendWelcome> ctx)
    {
        var m = ctx.Message;
        await Task.Delay(300);
        logger.LogInformation("Вітальний лист надіслано: {Email} ({Name}, {City})",
            m.Email, m.FullName, m.City);
        await ctx.Publish(new WelcomeSent(m.RegistrationId));
    }
}