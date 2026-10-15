namespace Shared.Contracts.Sagas;

public record RegistrationSubmitted(Guid RegistrationId, string FullName, string Email, string Phone, string City);

public record CreateProfile(Guid RegistrationId, string FullName, string Email, string Phone, string City);
public record ActivateProfile(Guid RegistrationId, int ClientId, string Email);
public record SendWelcome(Guid RegistrationId, string FullName, string Email, string City);
public record DeleteProfile(Guid RegistrationId, int ClientId);

public record ProfileCreated(Guid RegistrationId, int ClientId);
public record ProfileActivated(Guid RegistrationId);
public record ProfileActivationFailed(Guid RegistrationId, string Reason);
public record WelcomeSent(Guid RegistrationId);