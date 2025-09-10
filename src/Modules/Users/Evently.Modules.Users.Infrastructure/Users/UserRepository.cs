using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Evently.Modules.Users.Infrastructure.Users;
internal sealed class UserRepository(UsersDbContext usersDbContext) : IUserRepository
{
    private readonly UsersDbContext _usersDbContext = usersDbContext;

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _usersDbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Insert(User user)
    {
        _usersDbContext.Add(user);
    }
}
