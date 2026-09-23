using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IAccessService access,
    IClock clock) : IRequestHandler<CreateUserCommand, Guid>
{
      public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var needsStore = request.Role is UserRole.Staff or UserRole.StoreManager;
            if (me.Role == UserRole.AreaManager && !needsStore)
                  throw new ForbiddenException("Area managers can only create staff and store managers.");

            Guid? storeId = null;
            if (needsStore)
            {
                  storeId = request.StoreId ?? throw new BadRequestException("Choose a store for this role.");
                  await access.EnsureStoreAccessAsync(storeId.Value, cancellationToken);

                  _ = await unitOfWork.StoreRepository.GetByIdWithDevicesAsync(storeId.Value)
                      ?? throw new NotFoundException(nameof(Store), storeId.Value);
            }

            if (await unitOfWork.UserRepository.GetUserByEmail(request.Email, cancellationToken) is not null)
                  throw new ConflictException($"A user with email '{request.Email}' already exists.");

            if (await unitOfWork.UserRepository.GetUserByUsername(request.Username, cancellationToken) is not null)
                  throw new ConflictException($"A user with username '{request.Username}' already exists.");

            var user = new User(
                  request.Username.Trim(),
                  request.Email.Trim(),
                  passwordHasher.Hash(request.Password),
                  request.DisplayName.Trim(),
                  request.ProfileImageUrl?.Trim() ?? string.Empty,
                  request.Role)
            {
                  StoreId = storeId,
                  CreatedDate = clock.UtcNow
            };

            await unitOfWork.UserRepository.AddAsync(user, cancellationToken);
            await unitOfWork.Complete(cancellationToken);

            return user.Id;
      }
}
