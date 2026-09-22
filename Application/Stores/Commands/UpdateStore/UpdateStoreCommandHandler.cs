using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Commands.UpdateStore;

public class UpdateStoreCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateStoreCommand>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
      {
            var store = await _unitOfWork.StoreRepository.GetByIdWithRoutersAsync(request.Id)
                ?? throw new NotFoundException(nameof(Store), request.Id);

            foreach (var mac in request.RouterMacs)
            {
                  var existingStore = await _unitOfWork.StoreRepository.GetByRouterMacAsync(mac);
                  if (existingStore is not null && existingStore.Id != store.Id)
                        throw new ConflictException($"Router MAC '{mac}' is already registered to another store.");
            }

            if (request.AreaManagerId is not null)
            {
                  var areaManager = await _unitOfWork.UserRepository.GetUserByUserId(request.AreaManagerId.Value, cancellationToken)
                      ?? throw new NotFoundException(nameof(User), request.AreaManagerId.Value);

                  if (areaManager.DeleteDate is not null)
                        throw new NotFoundException(nameof(User), request.AreaManagerId.Value);
            }

            store.Name = request.Name;
            store.AreaManagerId = request.AreaManagerId;
            store.UpdateDate = DateTime.UtcNow;

            static string NormalizeMac(string mac) => mac.Trim().Replace("-", ":").ToUpperInvariant();

            var requestedMacs = request.RouterMacs
                .Where(mac => !string.IsNullOrWhiteSpace(mac))
                .Select(NormalizeMac)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var routersToRemove = store.RouterMacs
                .Where(router => !requestedMacs.Contains(NormalizeMac(router.MacAddress)))
                .ToList();

            foreach (var router in routersToRemove)
            {
                  store.RouterMacs.Remove(router);
            }

            foreach (var mac in requestedMacs)
            {
                  if (store.RouterMacs.Any(router => NormalizeMac(router.MacAddress) == mac))
                        continue;

                  store.RouterMacs.Add(new StoreRouter
                  {
                        Id = Guid.NewGuid(),
                        StoreId = store.Id,
                        Store = store,
                        MacAddress = mac,
                        CreatedDate = DateTime.UtcNow
                  });
            }

            await _unitOfWork.Complete(cancellationToken);
      }
}