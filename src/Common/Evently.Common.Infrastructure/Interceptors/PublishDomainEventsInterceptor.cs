using Evently.Common.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Common.Infrastructure.Outbox;

/// <summary>
/// Interceptor that automatically publishes domain events AFTER Entity Framework saves changes to the database.
/// This ensures domain events are published immediately after the database transaction is committed.
/// </summary>
public sealed class PublishDomainEventsInterceptor(IServiceScopeFactory serviceScopeFactory) : SaveChangesInterceptor
{
    /// <summary>
    /// Called after SaveChanges completes successfully. Publishes all domain events from tracked entities.
    /// </summary>
    /// <param name="eventData">Information about the save operation</param>
    /// <param name="result">Number of entities written to the database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result from the base implementation</returns>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        // Only publish domain events if we have a valid DbContext
        if (eventData.Context is not null)
        {
            await PublishDomainEventsAsync(eventData.Context);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Extracts domain events from all tracked entities, clears them, and publishes them using MediatR.
    /// </summary>
    /// <param name="context">The DbContext that was used for the save operation</param>
    private async Task PublishDomainEventsAsync(DbContext context)
    {
        // Extract all domain events from tracked entities
        var domainEvents = context
            .ChangeTracker
            .Entries<Entity>() // Get all tracked entities that inherit from Entity
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                // Get the domain events from each entity
                IReadOnlyCollection<IDomainEvent> domainEvents = entity.DomainEvents;

                // Clear the events from the entity to prevent duplicate publishing
                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        // Create a new service scope to resolve dependencies
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        // Get the MediatR publisher from the service container
        IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        // Publish each domain event individually
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}
