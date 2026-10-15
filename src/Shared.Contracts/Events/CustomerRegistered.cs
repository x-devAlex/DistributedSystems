namespace Shared.Contracts.Events;

public record CustomerRegistered(int CustomerId, string FullName, string Email, string City);