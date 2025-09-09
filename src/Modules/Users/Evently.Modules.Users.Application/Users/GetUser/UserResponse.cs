namespace Evently.Modules.Users.Application.Users.GetUser;
public sealed record UserResponse(Guid UserId, string Email, string FirstName, string LastName);
