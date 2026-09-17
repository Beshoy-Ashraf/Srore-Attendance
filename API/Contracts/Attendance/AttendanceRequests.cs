namespace API.Contracts.Attendance;

public record CheckInRequest(string RouterMac, string DeviceMac, string DeviceIp);

public record CheckOutRequest(string RouterMac, string DeviceMac);

public record ManualAttendanceRequest(
    Guid StaffId,
    DateOnly Date,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string? Notes);