using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.DeleteSchedule;

/// <summary>An explicit withdrawal by the store manager/admin who owns the schedule — distinct from an
/// Area Manager's reject, which is recorded with a <see cref="Schedule.RejectionReason"/>.</summary>
public class DeleteScheduleCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<DeleteScheduleCommand>
{
      public async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.StoreManager);

            var schedule = await unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            await access.EnsureStaffAccessAsync(schedule.StaffId, cancellationToken);

            if (schedule.Status == ScheduleStatus.Approved)
                  throw new BadRequestException("An approved schedule can't be deleted directly; ask the area manager to reject it.");

            schedule.DeletedDate = clock.UtcNow;
            schedule.UpdateDate = clock.UtcNow;

            await unitOfWork.Complete(cancellationToken);
      }
}
