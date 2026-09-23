using Application.Devices.Dtos;
using MediatR;

namespace Application.Devices.Queries.GetDeviceByStaff;

public record GetDeviceByStaffQuery(Guid StaffId) : IRequest<DeviceDto?>;
