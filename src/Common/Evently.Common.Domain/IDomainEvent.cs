using MediatR;

namespace Evently.Common.Domain;

// Allow to use Domain Events in the application
// Allow us to use publish subscribe functionality in mediatr library
public interface IDomainEvent : INotification
{
    Guid Id { get; }

    DateTime OccurredOnUtc { get; }
}
