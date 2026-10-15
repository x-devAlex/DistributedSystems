using MassTransit;
using NotificationService.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ProcessedMessageStore>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CustomerRegisteredConsumer>();

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
    });
});

var host = builder.Build();
host.Run();