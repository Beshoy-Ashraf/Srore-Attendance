namespace Application.AttendanceSettings.Dtos;

public record AttendanceSettingsDto(
    Guid Id,
    Guid StoreId,
    TimeOnly MorningStart,
    TimeOnly MorningEnd,
    TimeOnly NightStart,
    TimeOnly NightEnd,
    int LateGraceMinutes);