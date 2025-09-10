using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using Evently.Modules.Users.Application.Abstractions.Data;
using Evently.Modules.Users.Domain.Users;

namespace Evently.Modules.Users.Application.Users.UpdateProfile;
internal sealed class UpdateProfileCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetAsync(request.UserId, cancellationToken);

        if(user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        user.UpdateProfile(request.FirstName, request.LastName);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
