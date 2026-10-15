using MassTransit;
using Shared.Contracts.Sagas;

namespace SagaService.Sagas;

public class RegistrationStateMachine : MassTransitStateMachine<RegistrationState>
{
    public State AwaitingProfile { get; private set; } = null!;
    public State AwaitingActivation { get; private set; } = null!;
    public State AwaitingWelcome { get; private set; } = null!;

    public Event<RegistrationSubmitted> Submitted { get; private set; } = null!;
    public Event<ProfileCreated> ProfileOk { get; private set; } = null!;
    public Event<ProfileActivated> ActivationOk { get; private set; } = null!;
    public Event<ProfileActivationFailed> ActivationNo { get; private set; } = null!;
    public Event<WelcomeSent> WelcomeOk { get; private set; } = null!;

    public RegistrationStateMachine()
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => Submitted, x => x.CorrelateById(m => m.Message.RegistrationId));
        Event(() => ProfileOk, x => x.CorrelateById(m => m.Message.RegistrationId));
        Event(() => ActivationOk, x => x.CorrelateById(m => m.Message.RegistrationId));
        Event(() => ActivationNo, x => x.CorrelateById(m => m.Message.RegistrationId));
        Event(() => WelcomeOk, x => x.CorrelateById(m => m.Message.RegistrationId));

        Initially(
            When(Submitted)
                .Then(c =>
                {
                    c.Saga.FullName = c.Message.FullName;
                    c.Saga.Email = c.Message.Email;
                    c.Saga.Phone = c.Message.Phone;
                    c.Saga.City = c.Message.City;
                    Console.WriteLine($"[Saga {c.Saga.CorrelationId}] Initial -> AwaitingProfile");
                })
                .Send(c => new CreateProfile(c.Saga.CorrelationId, c.Saga.FullName,
                    c.Saga.Email, c.Saga.Phone, c.Saga.City))
                .TransitionTo(AwaitingProfile));

        During(AwaitingProfile,
            When(ProfileOk)
                .Then(c => c.Saga.ClientId = c.Message.ClientId)
                .Then(c => Console.WriteLine($"[Saga {c.Saga.CorrelationId}] профіль створено, id={c.Saga.ClientId}"))
                .Send(c => new ActivateProfile(c.Saga.CorrelationId, c.Saga.ClientId!.Value, c.Saga.Email))
                .TransitionTo(AwaitingActivation));

        During(AwaitingActivation,
            When(ActivationOk)
                .Then(c => Console.WriteLine($"[Saga {c.Saga.CorrelationId}] профіль активовано"))
                .Send(c => new SendWelcome(c.Saga.CorrelationId, c.Saga.FullName,
                    c.Saga.Email, c.Saga.City))
                .TransitionTo(AwaitingWelcome),
            When(ActivationNo)
                .Then(c => Console.WriteLine($"[Saga {c.Saga.CorrelationId}] активація не вдалася: {c.Message.Reason}. КОМПЕНСАЦІЯ"))
                .Send(c => new DeleteProfile(c.Saga.CorrelationId, c.Saga.ClientId!.Value))
                .Finalize());

        During(AwaitingWelcome,
            When(WelcomeOk)
                .Then(c => Console.WriteLine($"[Saga {c.Saga.CorrelationId}] лист надіслано, процес завершено"))
                .Finalize());

        SetCompletedWhenFinalized();
    }
}