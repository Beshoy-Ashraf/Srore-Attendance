using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock) : IRequestHandler<UpdateUserCommand>
{
      public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var target = await unitOfWork.UserRepository.GetActiveByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.Id);

            var needsStore = request.Role is UserRole.Staff or UserRole.StoreManager;
            Guid? newStoreId = needsStore ? request.StoreId : null;

            if (me.Id == target.Id)
            {
                  // Anyone may edit their own profile, but nobody can change their own role or store.
                  if (request.Role != target.Role || newStoreId != target.StoreId)
                        throw new ForbiddenException("You can't change your own role or store.");
            }
            else
            {
                  await access.EnsureCanManageUserAsync(target, cancellationToken);

                  if (me.Role == UserRole.AreaManager && !needsStore)
                        throw new ForbiddenException("Area managers can only assign the staff or store manager role.");
            }

            if (needsStore)
            {
                  var storeId = newStoreId ?? throw new BadRequestException("Choose a store for this role.");
                  await access.EnsureStoreAccessAsync(storeId, cancellationToken);

                  _ = await unitOfWork.StoreRepository.GetByIdWithDevicesAsync(storeId)
                      ?? throw new NotFoundException(nameof(Store), storeId);
            }

            target.DisplayName = request.DisplayName.Trim();
            target.ProfilePictureUrl = request.ProfilePictureUrl?.Trim() ?? string.Empty;
            target.Role = request.Role;
            target.StoreId = newStoreId;
            target.UpdateDate = clock.UtcNow;

            await unitOfWork.Complete(cancellationToken);
      }
}
