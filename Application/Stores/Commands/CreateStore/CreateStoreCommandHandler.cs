using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Commands.CreateStore;

public class CreateStoreCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<CreateStoreCommand, Guid>
{
      public async Task<Guid> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);

            var areaManagerId = request.AreaManagerId;
            if (me.Role == UserRole.AreaManager)
            {
                  if (areaManagerId is { } requested && requested != me.Id)
                        throw new ForbiddenException("Area managers can only create stores assigned to themselves.");
                  areaManagerId = me.Id;
            }

            if (areaManagerId is { } amId)
            {
                  var areaManager = await unitOfWork.UserRepository.GetUserByUserId(amId, cancellationToken);
                  if (areaManager is null || areaManager.DeleteDate is not null || areaManager.Role != UserRole.AreaManager)
                        throw new NotFoundException(nameof(User), amId);
            }



            var normalizedDeviceMacs = request.DeviceMacs.Select(MacAddress.Normalize).Distinct().ToList();
            foreach (var mac in normalizedDeviceMacs)
            {
                  if (await unitOfWork.StoreRepository.IsDeviceMacRegisteredAsync(mac))
                        throw new ConflictException($"Device MAC '{mac}' is already registered to another store.");
            }

            var now = clock.UtcNow;
            var store = new Store
            {
                  Id = Guid.NewGuid(),
                  Name = request.Name.Trim(),
                  AreaManagerId = areaManagerId,
                  CreatedDate = now,

                  Devices = normalizedDeviceMacs.Select(mac => new StoreDevice
                  {
                        Id = Guid.NewGuid(),
                        MacAddress = mac,
                        CreatedDate = now
                  }).ToList()
            };

            await unitOfWork.StoreRepository.AddAsync(store, cancellationToken);
            await unitOfWork.Complete(cancellationToken);

            return store.Id;
      }
}