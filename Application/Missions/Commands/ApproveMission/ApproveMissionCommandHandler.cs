using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Commands.ApproveMission;

public class ApproveMissionCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<ApproveMissionCommand>
{
      public async Task Handle(ApproveMissionCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var mission = await unitOfWork.MissionRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Mission), request.Id);

            await access.EnsureStaffAccessAsync(mission.StaffId, cancellationToken);

            if (mission.Status != RequestStatus.Pending)
                  throw new BadRequestException("Only pending missions can be approved.");

            mission.Status = RequestStatus.Approved;
            mission.ApprovedByAreaManagerId = me.Id;
            mission.ApprovedDate = clock.UtcNow;
            mission.UpdateDate = clock.UtcNow;

            await unitOfWork.Complete(cancellationToken);
      }
}
