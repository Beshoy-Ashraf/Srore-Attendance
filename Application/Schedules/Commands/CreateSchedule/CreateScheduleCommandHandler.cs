using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<CreateScheduleCommand, Guid>
{
      public async Task<Guid> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.StoreManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);
            var staff = await access.EnsureStaffAccessAsync(request.StaffId, cancellationToken);

            if (staff.StoreId is null)
                  throw new BadRequestException("This staff member isn't assigned to a store.");

            var existing = await unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(request.StaffId, request.Date);
            if (existing is not null)
                  throw new ConflictException("This staff member already has a schedule for that date.");

            var schedule = new Schedule
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  Date = request.Date,
                  ShiftType = request.ShiftType,
                  StartTime = request.StartTime,
                  EndTime = request.EndTime,
                  Status = ScheduleStatus.Pending,
                  CreatedByStoreManagerId = me.Id,
                  CreatedDate = clock.UtcNow
            };

            await unitOfWork.ScheduleRepository.AddAsync(schedule, cancellationToken);
            await unitOfWork.Complete(cancellationToken);

            return schedule.Id;
      }
}
