namespace Evently.Modules.Users.Application.Abstractions.Data;

internal interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
