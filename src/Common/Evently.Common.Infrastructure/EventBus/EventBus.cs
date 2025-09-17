using Evently.Common.Application.EventBus;
using MassTransit;

namespace Evently.Common.Infrastructure.EventBus;

internal sealed class EventBus(IBus bus) : IEventBus
{
    private readonly IBus _bus = bus;

    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        await _bus.Publish(integrationEvent, cancellationToken);
    }
}
