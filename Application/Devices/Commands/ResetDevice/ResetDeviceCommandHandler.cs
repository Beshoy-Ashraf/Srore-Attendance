using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Devices.Commands.ResetDevice;

public class ResetDeviceCommandHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<ResetDeviceCommand>
{
      public async Task Handle(ResetDeviceCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager, UserRole.StoreManager);
            await access.EnsureStaffAccessAsync(request.StaffId, cancellationToken);

            var device = await unitOfWork.DeviceRepository.GetByStaffIdAsync(request.StaffId);
            if (device is null)
                  return; // Nothing registered yet — the next check-in will register one.

            device.IsActive = false;

            await unitOfWork.Complete(cancellationToken);
      }
}
