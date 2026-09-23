using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Commands.RejectMission;

/// <summary>Rejecting a mission soft-deletes it, the same pattern used for schedules and requests.</summary>
public class RejectMissionCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<RejectMissionCommand>
{
      public async Task Handle(RejectMissionCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var mission = await unitOfWork.MissionRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Mission), request.Id);

            await access.EnsureStaffAccessAsync(mission.StaffId, cancellationToken);

            if (mission.Status != RequestStatus.Pending)
                  throw new BadRequestException("Only pending missions can be rejected.");

            var now = clock.UtcNow;
            mission.Status = RequestStatus.Rejected;
            mission.ApprovedByAreaManagerId = me.Id;
            mission.ApprovedDate = now;
            mission.UpdateDate = now;
            mission.RejectionReason = request.Reason;
            mission.DeletedDate = now;

            await unitOfWork.Complete(cancellationToken);
      }
}
