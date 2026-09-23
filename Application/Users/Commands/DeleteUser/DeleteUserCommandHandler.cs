using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock) : IRequestHandler<DeleteUserCommand>
{
      public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);
            if (me.Id == request.Id)
                  throw new BadRequestException("You can't delete your own account.");

            var user = await unitOfWork.UserRepository.GetUserByUserId(request.Id, cancellationToken);
            if (user is null || user.DeleteDate is not null)
                  throw new NotFoundException(nameof(User), request.Id);

            await access.EnsureCanManageUserAsync(user, cancellationToken);

            var now = clock.UtcNow;
            user.DeleteDate = now;
            user.UpdateDate = now;

            // A deleted account must not be able to keep a session alive.
            foreach (var token in user.RefreshTokens)
                  token.IsRevoked = true;

            // Stores keep pointing at nobody rather than at a deleted area manager.
            foreach (var store in await unitOfWork.StoreRepository.GetByAreaManagerAsync(user.Id, cancellationToken))
            {
                  store.AreaManagerId = null;
                  store.UpdateDate = now;
            }

            var device = await unitOfWork.DeviceRepository.GetByStaffIdAsync(user.Id);
            if (device is not null)
                  unitOfWork.DeviceRepository.DeleteAsync(device, cancellationToken);

            await unitOfWork.Complete(cancellationToken);
      }
}
