using MassTransit;
using NotificationService.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ProcessedMessageStore>();

builder.Services.AddMassTransit(x =>
{
    // консюмери ЛР5: подія CustomerRegistered
    x.AddConsumer<CustomerRegisteredConsumer>();

    // консюмер ЛР6: команда SendWelcome від саги
    x.AddConsumer<SendWelcomeConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("customer-registered", e =>
        {
            e.UseMessageRetry(r => r.Exponential(5,
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2)));
            e.ConfigureConsumer<CustomerRegisteredConsumer>(context);
        });

        cfg.ReceiveEndpoint("send-welcome", e =>
        {
            e.ConfigureConsumer<SendWelcomeConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();