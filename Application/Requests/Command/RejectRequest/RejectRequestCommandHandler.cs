using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Commands.RejectRequest;

/// <summary>Rejecting a request soft-deletes it, the same as a rejected schedule: it stops counting as a
/// live request while staying visible to the Area Manager as rejected history.</summary>
public class RejectRequestCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<RejectRequestCommand>
{
      public async Task Handle(RejectRequestCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var entity = await unitOfWork.RequestRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Request), request.Id);

            await access.EnsureStaffAccessAsync(entity.StaffId, cancellationToken);

            if (entity.Status != RequestStatus.Pending)
                  throw new BadRequestException("Only pending requests can be rejected.");

            var now = clock.UtcNow;
            entity.Status = RequestStatus.Rejected;
            entity.ApprovedByAreaManagerId = me.Id;
            entity.ApprovedDate = now;
            entity.UpdateDate = now;
            entity.RejectionReason = request.Reason;
            entity.DeletedDate = now;

            await unitOfWork.Complete(cancellationToken);
      }
}
