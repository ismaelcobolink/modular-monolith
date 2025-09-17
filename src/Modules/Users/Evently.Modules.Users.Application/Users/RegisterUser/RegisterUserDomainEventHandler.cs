using Evently.Common.Application.EventBus;
using Evently.Common.Application.Exceptions;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Users.Application.Users.GetUser;
using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.IntegrationEvents;
using MediatR;

namespace Evently.Modules.Users.Application.Users.RegisterUser;

internal sealed class RegisterUserDomainEventHandler(ISender sender, IEventBus eventBus) : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly ISender _sender = sender;
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await _sender.Send(new GetUserQuery(notification.UserId), cancellationToken);

        if (result.IsFailure)
        {
            throw new EventlyException(nameof(RegisterUserDomainEventHandler), result.Error);
        }

        await _eventBus.PublishAsync(
            new UserRegisteredIntegrationEvent(
                    notification.Id,
                    notification.OccurredOnUtc,
                    notification.UserId,
                    result.Value.Email,
                    result.Value.FirstName,
                    result.Value.LastName
                ), 
            cancellationToken);
    }
}
