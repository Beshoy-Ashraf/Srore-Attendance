using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Stores keep time in UTC but shifts, lateness and reports are about the store's wall clock.
/// The zone comes from <c>Attendance:TimeZoneId</c> (IANA id, default Africa/Cairo).
/// </summary>
public sealed class StoreClock : IClock
{
    private const string DefaultZoneId = "Africa/Cairo";
    private readonly TimeZoneInfo _zone;

    public StoreClock(IConfiguration configuration, ILogger<StoreClock> logger)
    {
        var zoneId = configuration["Attendance:TimeZoneId"];
        if (string.IsNullOrWhiteSpace(zoneId))
            zoneId = DefaultZoneId;

        try
        {
            _zone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            logger.LogWarning(ex, "Time zone '{ZoneId}' was not found on this machine; falling back to UTC. " +
                                  "Lateness and reports will be off by the store's UTC offset.", zoneId);
            _zone = TimeZoneInfo.Utc;
        }
    }

    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly LocalToday => ToLocalDate(UtcNow);

    public DateOnly ToLocalDate(DateTime utc) => DateOnly.FromDateTime(ToLocal(utc));

    public TimeOnly ToLocalTime(DateTime utc) => TimeOnly.FromDateTime(ToLocal(utc));

    public DateTime LocalToUtc(DateOnly date, TimeOnly time)
    {
        var local = DateTime.SpecifyKind(date.ToDateTime(time), DateTimeKind.Unspecified);

        // The hour skipped by a spring-forward transition does not exist on the wall clock.
        if (_zone.IsInvalidTime(local))
            local = local.AddHours(1);

        return TimeZoneInfo.ConvertTimeToUtc(local, _zone);
    }

    private DateTime ToLocal(DateTime utc)
    {
        var asUtc = utc.Kind == DateTimeKind.Local
            ? utc.ToUniversalTime()
            : DateTime.SpecifyKind(utc, DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(asUtc, _zone);
    }
}
