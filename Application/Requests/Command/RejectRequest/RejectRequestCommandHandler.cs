using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Commands.RejectRequest;

public class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public RejectRequestCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(RejectRequestCommand request, CancellationToken cancellationToken)
      {
            var entity = await _unitOfWork.RequestRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Request), request.Id);

            if (entity.Status != RequestStatus.Pending)
                  throw new BadRequestException("Only pending requests can be rejected.");

            entity.Status = RequestStatus.Rejected;
            entity.ApprovedByAreaManagerId = request.ApprovedByAreaManagerId;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.RequestRepository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}