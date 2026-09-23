using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IAccessService access,
    IPasswordHasher hasher,
    IClock clock) : IRequestHandler<ResetPasswordCommand>
{
      public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);
            if (me.Id == request.UserId)
                  throw new BadRequestException("Use change password for your own account.");

            var user = await unitOfWork.UserRepository.GetUserByUserId(request.UserId, cancellationToken);
            if (user is null || user.DeleteDate is not null)
                  throw new NotFoundException(nameof(User), request.UserId);

            await access.EnsureCanManageUserAsync(user, cancellationToken);

            user.PasswordHash = hasher.Hash(request.NewPassword);
            user.UpdateDate = clock.UtcNow;

            foreach (var token in user.RefreshTokens)
                  token.IsRevoked = true;

            await unitOfWork.Complete(cancellationToken);
      }
}
