using Evently.Common.Application.Messaging;
using Evently.Modules.Users.Domain.Users;

namespace Evently.Modules.Users.Application.Users.GetUser;
public sealed record GetUSerQuery(Guid UserId) : IQuery<User>;
