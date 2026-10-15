using MassTransit;

namespace SagaService.Sagas;

public class RegistrationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }          // = RegistrationId
    public string CurrentState { get; set; } = null!;
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string City { get; set; } = "";
    public int? ClientId { get; set; } 
}