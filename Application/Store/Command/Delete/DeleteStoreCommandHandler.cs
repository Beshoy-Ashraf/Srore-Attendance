using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Commands.DeleteStore;

public class DeleteStoreCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteStoreCommand>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
      {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Store), request.Id);

            store.DeleteDate = DateTime.UtcNow;

            await _unitOfWork.StoreRepository.UpdateAsync(store, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}