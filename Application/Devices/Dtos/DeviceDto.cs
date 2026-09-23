namespace Application.Devices.Dtos;

public record DeviceDto(
    Guid Id,
    Guid StaffId,
    string RegisteredDeviceMac,
    DateTime RegisteredDate,
    bool IsActive);
