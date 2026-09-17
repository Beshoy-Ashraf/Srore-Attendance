using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Commands.CheckOut;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, CheckOutResponseDto>
{
      private readonly IUnitOfWork _unitOfWork;

      public CheckOutCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<CheckOutResponseDto> Handle(CheckOutCommand request, CancellationToken cancellationToken)
      {
            var staff = await _unitOfWork.UserRepository.GetByIdAsync(request.StaffId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.User), request.StaffId);

            if (staff.StoreId is null)
                  throw new BadRequestException("Staff member is not assigned to a store.");

            var routerIsValid = await _unitOfWork.StoreRepository.HasRouterMacAsync(staff.StoreId.Value, request.RouterMac);
            if (!routerIsValid)
                  throw new BadRequestException("This router isn't registered to your store.");

            var device = await _unitOfWork.DeviceRepository.GetByStaffIdAsync(request.StaffId);
            if (device is null || !device.IsActive || device.RegisteredDeviceMac != request.DeviceMac)
            {
                  throw new BadRequestException(
                      "This PC isn't the one registered to your account. Ask your Store Manager to reset your registered device.");
            }

            var attendance = await _unitOfWork.AttendanceRepository.GetOpenAttendanceAsync(request.StaffId)
                ?? throw new BadRequestException("No open check-in found to check out from.");

            var checkOutTime = DateTime.UtcNow;
            attendance.CheckOutTime = checkOutTime;
            attendance.UpdateDate = checkOutTime;

            await _unitOfWork.AttendanceRepository.UpdateAsync(attendance, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            var hoursWorked = checkOutTime - attendance.CheckInTime!.Value;

            return new CheckOutResponseDto(
                attendance.Id,
                checkOutTime,
                hoursWorked,
                "Checked out successfully.");
      }
}