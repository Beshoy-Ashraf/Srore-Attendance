using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Commands.ManualAttendance;

public class ManualAttendanceCommandHandler : IRequestHandler<ManualAttendanceCommand, Guid>
{
      private readonly IUnitOfWork _unitOfWork;

      public ManualAttendanceCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<Guid> Handle(ManualAttendanceCommand request, CancellationToken cancellationToken)
      {
            var staff = await _unitOfWork.UserRepository.GetByIdAsync(request.StaffId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.User), request.StaffId);

            var schedule = await _unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(request.StaffId, request.Date);


            var existing = (await _unitOfWork.AttendanceRepository.GetByStaffAndDateRangeAsync(
                    request.StaffId, request.Date, request.Date))
                .FirstOrDefault();

            var checkInDateTime = request.CheckInTime is not null
                ? request.Date.ToDateTime(request.CheckInTime.Value, DateTimeKind.Utc)
                : existing?.CheckInTime;

            var checkOutDateTime = request.CheckOutTime is not null
                ? request.Date.ToDateTime(request.CheckOutTime.Value, DateTimeKind.Utc)
                : existing?.CheckOutTime;

            var isLate = schedule is not null
                && request.CheckInTime is not null
                && request.CheckInTime.Value > schedule.StartTime;

            if (existing is not null)
            {
                  existing.CheckInTime = checkInDateTime;
                  existing.CheckOutTime = checkOutDateTime;
                  existing.VerificationMethod = VerificationMethod.Manual;
                  existing.IsLate = isLate;
                  existing.EnteredManuallyBy = request.EnteredManuallyBy;
                  existing.Notes = request.Notes;
                  existing.UpdateDate = DateTime.UtcNow;

                  await _unitOfWork.AttendanceRepository.UpdateAsync(existing, cancellationToken);
                  await _unitOfWork.Complete(cancellationToken);

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
                  EnteredManuallyBy = request.EnteredManuallyBy,
                  Notes = request.Notes,
                  CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.AttendanceRepository.AddAsync(attendance, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            return attendance.Id;
      }
}