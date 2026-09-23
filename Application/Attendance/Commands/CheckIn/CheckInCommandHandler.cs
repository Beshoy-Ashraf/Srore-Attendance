using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Commands.CheckIn;

public class CheckInCommandHandler(IUnitOfWork unitOfWork, IClock clock) : IRequestHandler<CheckInCommand, CheckInResponseDto>
{
      public async Task<CheckInResponseDto> Handle(CheckInCommand request, CancellationToken cancellationToken)
      {
            if (!MacAddress.IsValid(request.DeviceMac) || !MacAddress.IsValid(request.DeviceMac))
                  throw new BadRequestException("A valid device and device MAC address are required.");

            var normalizedDeviceMac = MacAddress.Normalize(request.DeviceMac);

            var staff = await unitOfWork.UserRepository.GetActiveByIdAsync(request.StaffId, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.StaffId);

            if (staff.StoreId is null)
                  throw new BadRequestException("Staff member is not assigned to a store.");

            var deviceIsValid = await unitOfWork.StoreRepository.HasDeviceMacAsync(staff.StoreId.Value, normalizedDeviceMac);
            if (!deviceIsValid)
                  throw new BadRequestException("This device isn't registered to your store.");

            var device = await unitOfWork.DeviceRepository.GetByStaffIdAsync(request.StaffId);

            if (device is null)
            {
                  device = new Device
                  {
                        Id = Guid.NewGuid(),
                        StaffId = request.StaffId,
                        RegisteredDeviceMac = normalizedDeviceMac,
                        RegisteredDate = clock.UtcNow,
                        IsActive = true
                  };
                  await unitOfWork.DeviceRepository.AddAsync(device, cancellationToken);
            }
            else if (!device.IsActive || device.RegisteredDeviceMac != normalizedDeviceMac)
            {
                  throw new BadRequestException(
                      "This PC isn't the one registered to your account. Ask your Store Manager to reset your registered device.");
            }

            var today = clock.LocalToday;
            var schedule = await unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(request.StaffId, today);
            var settings = await unitOfWork.AttendanceSettingsRepository.GetByStoreIdAsync(staff.StoreId.Value);

            var checkInTime = clock.UtcNow;
            var isLate = schedule is not null && AttendanceRules.IsWorkingShift(schedule.ShiftType)
                && AttendanceRules.IsLate(clock.ToLocalTime(checkInTime), schedule.StartTime, settings?.LateGraceMinutes ?? 0);

            var attendance = new Domain.Entities.Attendance
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  ScheduleId = schedule?.Id,
                  CheckInTime = checkInTime,
                  CheckInDeviceMac = normalizedDeviceMac,
                  VerificationMethod = VerificationMethod.Network,
                  IsLate = isLate,
                  IsDeviceMismatch = false,
                  CreatedDate = checkInTime
            };

            await unitOfWork.AttendanceRepository.AddAsync(attendance, cancellationToken);
            await unitOfWork.Complete(cancellationToken);

            return new CheckInResponseDto(
                attendance.Id,
                checkInTime,
                isLate,
                isLate ? "Checked in — marked late." : "Checked in successfully.");
      }
}
