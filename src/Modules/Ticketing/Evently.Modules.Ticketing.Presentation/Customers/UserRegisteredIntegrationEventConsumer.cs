using Evently.Common.Application.Exceptions;
using Evently.Common.Domain;
using Evently.Modules.Ticketing.Application.Customers.CreateCustomer;
using Evently.Modules.Users.IntegrationEvents;
using MassTransit;
using MediatR;

namespace Evently.Modules.Ticketing.Presentation.Customers;

public sealed class UserRegisteredIntegrationEventConsumer(ISender sender) : IConsumer<UserRegisteredIntegrationEvent>
{
    private readonly ISender _sender = sender;

    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        Result result = await _sender.Send(
            new CreateCustomerCommand(
                context.Message.UserId, 
                context.Message.Email, 
                context.Message.FirstName, 
                context.Message.LastName));

        if (result.IsFailure)
        {
            throw new EventlyException(nameof(UserRegisteredIntegrationEventConsumer), result.Error);
        }
    }
}
