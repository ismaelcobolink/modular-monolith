using Evently.Common.Application.Messaging;

namespace Evently.Modules.Users.Application.Users.UpdateProfile;
public sealed record UpdateProfileCommand(Guid UserId, string FirstName, string LastName) : ICommand;
