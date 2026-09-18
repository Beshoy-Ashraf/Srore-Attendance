using MediatR;

namespace Application.AttendanceSettings.Commands.UpsertAttendanceSettings;

public record UpsertAttendanceSettingsCommand(
    Guid StoreId,
    TimeOnly MorningStart,
    TimeOnly MorningEnd,
    TimeOnly NightStart,
    TimeOnly NightEnd,
    int LateGraceMinutes) : IRequest<Guid>;