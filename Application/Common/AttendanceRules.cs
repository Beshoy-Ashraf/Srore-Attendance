using Domain.Enums;

namespace Application.Common;

/// <summary>Business rules that more than one feature depends on.</summary>
public static class AttendanceRules
{
    /// <summary>An Area Manager is alerted about unapproved schedules for a month this many days before it starts.</summary>
    public const int ScheduleAlertLeadDays = 7;

    public const int MaxReportDays = 93;

    public static bool IsWorkingShift(ShiftType type) => type is ShiftType.AM or ShiftType.PM or ShiftType.FULL or ShiftType.BW;

    public static bool IsLeaveShift(ShiftType type) => type is ShiftType.ANN or ShiftType.SL or ShiftType.OFF;

    /// <summary>Late when the check-in is after the shift start plus the store's grace period (whole minutes, no midnight wrap).</summary>
    public static bool IsLate(TimeOnly localCheckIn, TimeOnly scheduledStart, int graceMinutes) =>
        Minutes(localCheckIn) > Minutes(scheduledStart) + Math.Max(0, graceMinutes);

    private static int Minutes(TimeOnly time) => time.Hour * 60 + time.Minute;
}
