using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.RejectSchedule;

/// <summary>Rejecting a schedule soft-deletes it: it stops counting as the staff member's live schedule,
/// while staying visible to the Area Manager as rejected history (see IScheduleRepository.GetPagedAsync).</summary>
public class RejectScheduleCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<RejectScheduleCommand>
{
      public async Task Handle(RejectScheduleCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var schedule = await unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            await access.EnsureStaffAccessAsync(schedule.StaffId, cancellationToken);

            if (schedule.Status != ScheduleStatus.Pending)
                  throw new BadRequestException("Only pending schedules can be rejected.");

            var now = clock.UtcNow;
            schedule.Status = ScheduleStatus.Rejected;
            schedule.ApprovedByAreaManagerId = me.Id;
            schedule.ApprovedDate = now;
            schedule.UpdateDate = now;
            schedule.RejectionReason = request.Reason;
            schedule.DeletedDate = now;

            await unitOfWork.Complete(cancellationToken);
      }
}
