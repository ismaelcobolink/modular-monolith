using Evently.Common.Domain;

namespace Evently.Modules.Events.Domain.TicketTypes;

public static class TicketTypeErrors
{
    public static Error NotFound(Guid ticketTypeId) =>
        Error.NotFound("TicketTypes.NotFound", $"The ticket type with the identifier {ticketTypeId} was not found");

    public static Error SamePrice() =>
        Error.Problem("TicketTypes.SamePrice", "The new price is the same as the current price");

    public static Error CannotChangePriceAfterEventCompleted() =>
        Error.Problem(
            "TicketTypes.CannotChangePriceAfterEventCompleted",
            "The ticket type price cannot be changed after the event is completed");

    public static Error CannotChangePriceWithin24Hours() =>
        Error.Problem(
            "TicketTypes.CannotChangePriceWithin24Hours",
            "The ticket type price cannot be changed within 24 hours of the event start time");
}
