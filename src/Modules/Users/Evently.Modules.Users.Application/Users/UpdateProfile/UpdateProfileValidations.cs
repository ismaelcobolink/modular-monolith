using FluentValidation;

namespace Evently.Modules.Users.Application.Users.UpdateProfile;
internal sealed class UpdateProfileValidations : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidations()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(u => u.FirstName).NotEmpty();
        RuleFor(u => u.LastName).NotEmpty();
    }
}
