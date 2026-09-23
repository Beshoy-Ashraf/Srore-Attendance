using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Commands.CheckOut;

public class CheckOutCommandHandler(IUnitOfWork unitOfWork, IClock clock) : IRequestHandler<CheckOutCommand, CheckOutResponseDto>
{
      public async Task<CheckOutResponseDto> Handle(CheckOutCommand request, CancellationToken cancellationToken)
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
            if (device is null || !device.IsActive || device.RegisteredDeviceMac != normalizedDeviceMac)
            {
                  throw new BadRequestException(
                      "This PC isn't the one registered to your account. Ask your Store Manager to reset your registered device.");
            }

            var attendance = await unitOfWork.AttendanceRepository.GetOpenAttendanceAsync(request.StaffId)
                ?? throw new BadRequestException("No open check-in found to check out from.");

            var checkOutTime = clock.UtcNow;
            attendance.CheckOutTime = checkOutTime;
            attendance.UpdateDate = checkOutTime;

            await unitOfWork.Complete(cancellationToken);

            var hoursWorked = checkOutTime - attendance.CheckInTime!.Value;

            return new CheckOutResponseDto(
                attendance.Id,
                checkOutTime,
                hoursWorked,
                "Checked out successfully.");
      }
}
