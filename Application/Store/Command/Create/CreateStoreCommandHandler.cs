using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Commands.CreateStore;

public class CreateStoreCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateStoreCommand, Guid>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task<Guid> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
      {
            foreach (var mac in request.RouterMacs)
            {
                  var existingStore = await _unitOfWork.StoreRepository.GetByRouterMacAsync(mac);
                  if (existingStore is not null)
                        throw new ConflictException($"Router MAC '{mac}' is already registered to another store.");
            }

            var store = new Store
            {
                  Id = Guid.NewGuid(),
                  Name = request.Name,
                  AreaManagerId = request.AreaManagerId,
                  CreatedDate = DateTime.UtcNow,
                  RouterMacs = request.RouterMacs.Select(mac => new StoreRouter
                  {
                        Id = Guid.NewGuid(),
                        MacAddress = mac,
                        CreatedDate = DateTime.UtcNow
                  }).ToList()
            };

            await _unitOfWork.StoreRepository.AddAsync(store, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            return store.Id;
      }
}