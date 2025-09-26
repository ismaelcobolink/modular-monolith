using Evently.Common.Domain;
using Evently.Modules.Events.Domain.Events;

namespace Evently.Modules.Events.Domain.TicketTypes;

public sealed class TicketType : Entity
{
    private TicketType()
    {
    }

    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public string Name { get; private set; }

    public decimal Price { get; private set; }

    public string Currency { get; private set; }

    public decimal Quantity { get; private set; }

    // Solo el dominio puede crear instancias
    // Es decir, solo se puede crear a través del método Create en la clase Event
    internal static TicketType Create(
        Event @event,
        string name,
        decimal price,
        string currency,
        decimal quantity)
    {
        var ticketType = new TicketType
        {
            Id = Guid.NewGuid(),
            EventId = @event.Id,
            Name = name,
            Price = price,
            Currency = currency,
            Quantity = quantity
        };

        return ticketType;
    }

    public Result UpdatePrice(decimal price, Event @event)
    {
        if (Price == price)
        {
            return Result.Failure(TicketTypeErrors.SamePrice());
        }

        if (@event.Status == EventStatus.Completed)
        {
            return Result.Failure(TicketTypeErrors.CannotChangePriceAfterEventCompleted());
        }

        if (@event.StartsAtUtc <= DateTime.UtcNow.AddHours(24))
        {
            return Result.Failure(TicketTypeErrors.CannotChangePriceWithin24Hours());
        }

        Price = price;

        Raise(new TicketTypePriceChangedDomainEvent(Id, Price));

        return Result.Success();
    }
}
