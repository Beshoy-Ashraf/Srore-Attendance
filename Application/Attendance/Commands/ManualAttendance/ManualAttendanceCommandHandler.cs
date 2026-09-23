using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Commands.ManualAttendance;

public class ManualAttendanceCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<ManualAttendanceCommand, Guid>
{
      public async Task<Guid> Handle(ManualAttendanceCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager, UserRole.StoreManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            // Staff must be in the caller's own store(s) — a Store Manager can't enter attendance for another store.
            await access.EnsureStaffAccessAsync(request.StaffId, cancellationToken);

            var schedule = await unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(request.StaffId, request.Date);

            var dayStartUtc = clock.LocalToUtc(request.Date, TimeOnly.MinValue);
            var dayEndUtc = clock.LocalToUtc(request.Date.AddDays(1), TimeOnly.MinValue);
            var existing = (await unitOfWork.AttendanceRepository.GetInRangeAsync(
                    new[] { request.StaffId }, dayStartUtc, dayEndUtc, cancellationToken))
                .FirstOrDefault();

            var checkInDateTime = request.CheckInTime is not null
                ? clock.LocalToUtc(request.Date, request.CheckInTime.Value)
                : existing?.CheckInTime;

            var checkOutDateTime = request.CheckOutTime is not null
                ? clock.LocalToUtc(request.Date, request.CheckOutTime.Value)
                : existing?.CheckOutTime;

            var isLate = schedule is not null
                && AttendanceRules.IsWorkingShift(schedule.ShiftType)
                && request.CheckInTime is not null
                && request.CheckInTime.Value > schedule.StartTime;

            var now = clock.UtcNow;

            if (existing is not null)
            {
                  existing.CheckInTime = checkInDateTime;
                  existing.CheckOutTime = checkOutDateTime;
                  existing.VerificationMethod = VerificationMethod.Manual;
                  existing.IsLate = isLate;
                  existing.EnteredManuallyBy = me.Id;
                  existing.Notes = request.Notes;
                  existing.UpdateDate = now;

                  await unitOfWork.Complete(cancellationToken);

                  return existing.Id;
            }

            var attendance = new Domain.Entities.Attendance
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  ScheduleId = schedule?.Id,
                  CheckInTime = checkInDateTime,
                  CheckOutTime = checkOutDateTime,
                  VerificationMethod = VerificationMethod.Manual,
                  IsLate = isLate,
                  EnteredManuallyBy = me.Id,
                  Notes = request.Notes,
                  CreatedDate = now
            };

            await unitOfWork.AttendanceRepository.AddAsync(attendance, cancellationToken);
            await unitOfWork.Complete(cancellationToken);

            return attendance.Id;
      }
}
