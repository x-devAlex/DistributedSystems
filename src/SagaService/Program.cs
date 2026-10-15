using MassTransit;
using SagaService.Sagas;
using Shared.Contracts.Sagas;

var builder = WebApplication.CreateBuilder(args);

EndpointConvention.Map<CreateProfile>(new Uri("queue:create-profile"));
EndpointConvention.Map<ActivateProfile>(new Uri("queue:activate-profile"));
EndpointConvention.Map<DeleteProfile>(new Uri("queue:delete-profile"));
EndpointConvention.Map<SendWelcome>(new Uri("queue:send-welcome"));

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<RegistrationStateMachine, RegistrationState>()
        .InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapPost("/api/registrations", async (RegistrationRequest req, IPublishEndpoint publish) =>
{
    var id = Guid.NewGuid();
    await publish.Publish(new RegistrationSubmitted(id, req.FullName, req.Email, req.Phone, req.City));
    return Results.Accepted($"/api/registrations/{id}", new { registrationId = id });
});

app.Run();

public record RegistrationRequest(string FullName, string Email, string Phone, string City);