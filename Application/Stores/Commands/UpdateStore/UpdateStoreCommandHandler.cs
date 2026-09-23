using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Commands.UpdateStore;

public class UpdateStoreCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<UpdateStoreCommand>
{
      public async Task Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);
            await access.EnsureStoreAccessAsync(request.Id, cancellationToken);

            var store = await unitOfWork.StoreRepository.GetByIdWithDevicesAsync(request.Id)
                ?? throw new NotFoundException(nameof(Store), request.Id);

            // Only an admin may hand a store to a different area manager; an area manager may keep or clear their own.
            if (request.AreaManagerId != store.AreaManagerId && me.Role != UserRole.Admin)
                  throw new ForbiddenException("Only an admin can reassign a store's area manager.");

            // Only an admin manages a store's device fleet — a store manager/area manager can't add
            // rogue check-in devices.
            var normalizedDeviceMacs = request.DevicesMacs.Select(MacAddress.Normalize).Distinct().ToList();
            var currentDeviceMacs = store.Devices.Select(d => MacAddress.Normalize(d.MacAddress)).OrderBy(x => x).ToList();
            var deviceMacsChanged = !normalizedDeviceMacs.OrderBy(x => x).SequenceEqual(currentDeviceMacs);
            if (deviceMacsChanged && me.Role != UserRole.Admin)
                  throw new ForbiddenException("Only an admin can manage a store's registered devices.");

            if (request.AreaManagerId is { } amId)
            {
                  var areaManager = await unitOfWork.UserRepository.GetUserByUserId(amId, cancellationToken);
                  if (areaManager is null || areaManager.DeleteDate is not null || areaManager.Role != UserRole.AreaManager)
                        throw new NotFoundException(nameof(User), amId);
            }



            foreach (var mac in normalizedDeviceMacs)
            {
                  if (await unitOfWork.StoreRepository.IsDeviceMacRegisteredAsync(mac, excludeStoreId: store.Id))
                        throw new ConflictException($"Device MAC '{mac}' is already registered to another store.");
            }

            store.Name = request.Name.Trim();
            store.AreaManagerId = request.AreaManagerId;
            store.UpdateDate = clock.UtcNow;

            SyncDevices(store, normalizedDeviceMacs, clock);

            await unitOfWork.Complete(cancellationToken);
      }



      private static void SyncDevices(Store store, List<string> desiredMacs, IClock clock)
      {
            var toRemove = store.Devices
                .Where(d => !desiredMacs.Contains(MacAddress.Normalize(d.MacAddress)))
                .ToList();
            foreach (var device in toRemove)
                  store.Devices.Remove(device);

            foreach (var mac in desiredMacs)
            {
                  if (store.Devices.Any(d => MacAddress.Normalize(d.MacAddress) == mac))
                        continue;

                  store.Devices.Add(new StoreDevice
                  {
                        Id = Guid.NewGuid(),
                        StoreId = store.Id,
                        Store = store,
                        MacAddress = mac,
                        CreatedDate = clock.UtcNow
                  });
            }
      }
}