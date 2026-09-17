using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateRequestCommand, Guid>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task<Guid> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
      {
            var entity = new Request
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  RequestedById = request.RequestedById,
                  Type = request.Type,
                  DateFrom = request.DateFrom,
                  DateTo = request.DateTo,
                  Reason = request.Reason,
                  Status = RequestStatus.Pending,
                  CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.RequestRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            return entity.Id;
      }
}