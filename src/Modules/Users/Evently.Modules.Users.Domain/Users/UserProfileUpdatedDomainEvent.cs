using Evently.Common.Domain;

namespace Evently.Modules.Users.Domain.Users;
public sealed class UserProfileUpdatedDomainEvent(string firstName, string lastName) : DomainEvent
{
    public string FirstName { get; init; } = firstName;
    public string LastName { get; init; } = lastName;
}
