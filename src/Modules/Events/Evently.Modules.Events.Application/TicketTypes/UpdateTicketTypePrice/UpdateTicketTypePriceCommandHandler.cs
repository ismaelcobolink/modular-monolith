using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Domain.TicketTypes;

namespace Evently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;

internal sealed class UpdateTicketTypePriceCommandHandler(
    ITicketTypeRepository ticketTypeRepository,
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTicketTypePriceCommand>
{
    public async Task<Result> Handle(UpdateTicketTypePriceCommand request, CancellationToken cancellationToken)
    {
        TicketType? ticketType = await ticketTypeRepository.GetAsync(request.TicketTypeId, cancellationToken);

        if (ticketType is null)
        {
            return Result.Failure(TicketTypeErrors.NotFound(request.TicketTypeId));
        }

        Event? @event = await eventRepository.GetAsync(ticketType.EventId, cancellationToken);

        if(@event is null)
        {
            return Result.Failure(EventErrors.NotFound(ticketType.EventId));
        }

        ticketType.UpdatePrice(request.Price, @event);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
