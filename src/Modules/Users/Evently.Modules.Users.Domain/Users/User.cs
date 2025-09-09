using Evently.Common.Domain;

namespace Evently.Modules.Users.Domain.Users;

public sealed class User : Entity
{
    private User()
    {
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public static User Create(Guid id, string email, string firstName, string lastName)
    {
        User user = new()
        {
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
        /// You can not call Raise directly because it's protected and the create method is static.
        /// And if you try to call it Raise(new UserRegisteredDomainEvent(user.Id)); it wont work because it's not in the context of an instance.
        /// Raise(new UserRegisteredDomainEvent(user.Id));

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        if(string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return;
        }

        FirstName = firstName;
        LastName = lastName;

        // You can call Raise here because it's not static and it's in the context of an instance.
        Raise(new UserProfileUpdatedDomainEvent(firstName, lastName));
    }
}
