using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Commands.CheckIn;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, CheckInResponseDto>
{
      private readonly IUnitOfWork _unitOfWork;

      public CheckInCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<CheckInResponseDto> Handle(CheckInCommand request, CancellationToken cancellationToken)
      {
            var staff = await _unitOfWork.UserRepository.GetByIdAsync(request.StaffId, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.StaffId);

            if (staff.StoreId is null)
                  throw new BadRequestException("Staff member is not assigned to a store.");

            var routerIsValid = await _unitOfWork.StoreRepository.HasRouterMacAsync(staff.StoreId.Value, request.RouterMac);
            if (!routerIsValid)
                  throw new BadRequestException("This router isn't registered to your store.");

            var device = await _unitOfWork.DeviceRepository.GetByStaffIdAsync(request.StaffId);

            if (device is null)
            {
                  device = new Device
                  {
                        Id = Guid.NewGuid(),
                        StaffId = request.StaffId,
                        RegisteredDeviceMac = request.DeviceMac,
                        RegisteredDeviceIp = request.DeviceIp,
                        RegisteredDate = DateTime.UtcNow,
                        IsActive = true
                  };
                  await _unitOfWork.DeviceRepository.AddAsync(device, cancellationToken);
            }
            else
            {
                  var macMatches = device.IsActive && device.RegisteredDeviceMac == request.DeviceMac;

                  if (!macMatches)
                  {
                        throw new BadRequestException(
                            "This PC isn't the one registered to your account. Ask your Store Manager to reset your registered device.");
                  }

                  if (device.RegisteredDeviceIp != request.DeviceIp)
                  {
                        device.RegisteredDeviceIp = request.DeviceIp;
                        await _unitOfWork.DeviceRepository.UpdateAsync(device, cancellationToken);
                  }
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var schedule = await _unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(request.StaffId, today);

            var checkInTime = DateTime.UtcNow;
            var isLate = schedule is not null && TimeOnly.FromDateTime(checkInTime) > schedule.StartTime;

            var attendance = new Domain.Entities.Attendance
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  ScheduleId = schedule?.Id,
                  CheckInTime = checkInTime,
                  CheckInRouterMac = request.RouterMac,
                  CheckInDeviceMac = request.DeviceMac,
                  CheckInIp = request.DeviceIp,
                  VerificationMethod = VerificationMethod.Network,
                  IsLate = isLate,
                  IsDeviceMismatch = false,
                  CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.AttendanceRepository.AddAsync(attendance, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            return new CheckInResponseDto(
                attendance.Id,
                checkInTime,
                isLate,
                isLate ? "Checked in — marked late." : "Checked in successfully.");
      }
}