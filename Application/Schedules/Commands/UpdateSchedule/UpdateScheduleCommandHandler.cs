using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.UpdateSchedule;

public class UpdateScheduleCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<UpdateScheduleCommand>
{
      public async Task Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.StoreManager);

            var schedule = await unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            await access.EnsureStaffAccessAsync(schedule.StaffId, cancellationToken);

            // A rejected schedule is soft-deleted, so GetByIdAsync (filtered) would already have thrown NotFound for it;
            // reaching here always means a live Pending or Approved schedule.
            schedule.ShiftType = request.ShiftType;
            schedule.StartTime = request.StartTime;
            schedule.EndTime = request.EndTime;
            schedule.UpdateDate = clock.UtcNow;

            // Any change to an already-decided schedule needs to go back through approval.
            schedule.Status = ScheduleStatus.Pending;
            schedule.ApprovedByAreaManagerId = null;
            schedule.ApprovedDate = null;

            await unitOfWork.Complete(cancellationToken);
      }
}
