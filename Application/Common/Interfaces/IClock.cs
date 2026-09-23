namespace Application.Common.Interfaces;

/// <summary>
/// Time in UTC for storage, and in the store's wall-clock time for shifts, lateness and reports.
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }

    /// <summary>Today's date on the store's wall clock.</summary>
    DateOnly LocalToday { get; }

    DateOnly ToLocalDate(DateTime utc);

    TimeOnly ToLocalTime(DateTime utc);

    /// <summary>Converts a store wall-clock date and time to a UTC instant.</summary>
    DateTime LocalToUtc(DateOnly date, TimeOnly time);
}
