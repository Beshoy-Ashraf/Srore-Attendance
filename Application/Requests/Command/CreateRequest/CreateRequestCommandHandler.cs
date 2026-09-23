using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<CreateRequestCommand, Guid>
{
      public async Task<Guid> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);
            await access.EnsureStaffAccessAsync(request.StaffId, cancellationToken);

            if (await unitOfWork.RequestRepository.HasOverlapAsync(
                    request.StaffId, request.Type, request.DateFrom, request.DateTo, excludeId: null, cancellationToken))
                  throw new ConflictException("There's already a pending or approved request of this type covering that range.");

            var entity = new Request
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  RequestedById = me.Id,
                  Type = request.Type,
                  DateFrom = request.DateFrom,
                  DateTo = request.DateTo,
                  Reason = request.Reason,
                  Status = RequestStatus.Pending,
                  CreatedDate = clock.UtcNow
            };

            await unitOfWork.RequestRepository.AddAsync(entity, cancellationToken);
            await unitOfWork.Complete(cancellationToken);

            return entity.Id;
      }
}
