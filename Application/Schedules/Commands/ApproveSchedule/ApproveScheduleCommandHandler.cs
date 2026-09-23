using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.ApproveSchedule;

public class ApproveScheduleCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<ApproveScheduleCommand>
{
      public async Task Handle(ApproveScheduleCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var schedule = await unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            await access.EnsureStaffAccessAsync(schedule.StaffId, cancellationToken);

            if (schedule.Status != ScheduleStatus.Pending)
                  throw new BadRequestException("Only pending schedules can be approved.");

            schedule.Status = ScheduleStatus.Approved;
            schedule.ApprovedByAreaManagerId = me.Id;
            schedule.ApprovedDate = clock.UtcNow;
            schedule.UpdateDate = clock.UtcNow;

            await unitOfWork.Complete(cancellationToken);
      }
}
