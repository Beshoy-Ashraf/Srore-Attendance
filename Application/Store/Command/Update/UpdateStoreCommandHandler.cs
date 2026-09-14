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

            // Make sure none of the new MACs belong to a different store
            foreach (var mac in request.RouterMacs)
            {
                  var existingStore = await _unitOfWork.StoreRepository.GetByRouterMacAsync(mac);
                  if (existingStore is not null && existingStore.Id != store.Id)
                        throw new ConflictException($"Router MAC '{mac}' is already registered to another store.");
            }

            store.Name = request.Name;
            store.AreaManagerId = request.AreaManagerId;
            store.UpdateDate = DateTime.UtcNow;

            // Reconcile the router MAC collection: drop ones no longer present, add new ones
            var existingMacs = store.RouterMacs.Select(r => r.MacAddress).ToList();

            store.RouterMacs = store.RouterMacs
                .Where(r => request.RouterMacs.Contains(r.MacAddress))
                .ToList();

            foreach (var mac in request.RouterMacs.Except(existingMacs))
            {
                  store.RouterMacs.Add(new StoreRouter
                  {
                        Id = Guid.NewGuid(),
                        StoreId = store.Id,
                        MacAddress = mac,
                        CreatedDate = DateTime.UtcNow
                  });
            }

            await _unitOfWork.StoreRepository.UpdateAsync(store, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}