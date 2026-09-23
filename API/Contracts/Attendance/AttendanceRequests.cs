namespace API.Contracts.Attendance;

public record CheckInRequest(string DeviceMac);

public record CheckOutRequest( string DeviceMac);

public record ManualAttendanceRequest(
    Guid StaffId,
    DateOnly Date,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string? Notes);