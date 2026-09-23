using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IAccessService access,
    IPasswordHasher hasher,
    IClock clock) : IRequestHandler<ChangePasswordCommand>
{
      public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var user = await unitOfWork.UserRepository.GetUserByUserId(me.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), me.Id);

            if (!hasher.Verify(request.CurrentPassword, user.PasswordHash))
                  throw new BadRequestException("The current password is incorrect.");

            user.PasswordHash = hasher.Hash(request.NewPassword);
            user.UpdateDate = clock.UtcNow;

            // Signs out every other session.
            foreach (var token in user.RefreshTokens)
                  token.IsRevoked = true;

            await unitOfWork.Complete(cancellationToken);
      }
}
