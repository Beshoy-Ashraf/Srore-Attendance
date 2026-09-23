using Application.Common.Interfaces;
using Application.Devices.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Devices.Queries.GetDeviceByStaff;

public class GetDeviceByStaffQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetDeviceByStaffQuery, DeviceDto?>
{
      public async Task<DeviceDto?> Handle(GetDeviceByStaffQuery request, CancellationToken cancellationToken)
      {
            await access.EnsureStaffAccessAsync(request.StaffId, cancellationToken);

            var device = await unitOfWork.DeviceRepository.GetByStaffIdAsync(request.StaffId);

            return device is null
                ? null
                : new DeviceDto(device.Id, device.StaffId, device.RegisteredDeviceMac, device.RegisteredDate, device.IsActive);
      }
}
