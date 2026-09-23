using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Commands.DeleteStore;

public class DeleteStoreCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<DeleteStoreCommand>
{
      public async Task Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureStoreAccessAsync(request.Id, cancellationToken);

            var store = await unitOfWork.StoreRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Store), request.Id);

            if (await unitOfWork.StoreRepository.HasActiveStaffAsync(request.Id, cancellationToken))
                  throw new ConflictException("This store still has active staff assigned to it. Reassign or remove them first.");

            store.DeleteDate = clock.UtcNow;
            store.UpdateDate = clock.UtcNow;

            await unitOfWork.Complete(cancellationToken);
      }
}
