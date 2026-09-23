using Domain.Enums;

namespace Domain.Models;

// StoreIds semantics everywhere: null = not restricted to any store, empty = matches nothing.

public sealed record UserFilter
{
    public IReadOnlyCollection<Guid>? StoreIds { get; init; }

    /// <summary>Users that are always included even when they are outside <see cref="StoreIds"/> (self, area managers).</summary>
    public IReadOnlyCollection<Guid>? AlsoIncludeUserIds { get; init; }

    public UserRole? Role { get; init; }
    public IReadOnlyCollection<UserRole>? Roles { get; init; }
    public Guid? OnlyUserId { get; init; }
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record ScheduleFilter
{
    public Guid? StaffId { get; init; }
    public IReadOnlyCollection<Guid>? StoreIds { get; init; }
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public ScheduleStatus? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record RequestFilter
{
    public Guid? StaffId { get; init; }
    public IReadOnlyCollection<Guid>? StoreIds { get; init; }
    public RequestType? Type { get; init; }
    public RequestStatus? Status { get; init; }
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record MissionFilter
{
    public Guid? StaffId { get; init; }
    public IReadOnlyCollection<Guid>? StoreIds { get; init; }
    public RequestStatus? Status { get; init; }
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record AttendanceFilter
{
    public Guid? StaffId { get; init; }
    public IReadOnlyCollection<Guid>? StoreIds { get; init; }

    /// <summary>Inclusive lower bound (UTC) on the check-in instant.</summary>
    public DateTime? FromUtc { get; init; }

    /// <summary>Exclusive upper bound (UTC) on the check-in instant.</summary>
    public DateTime? ToUtcExclusive { get; init; }

    public bool? LateOnly { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>A pending schedule reduced to what the month-approval alert needs.</summary>
public sealed record PendingScheduleSlice(Guid StoreId, string StoreName, DateOnly Date);
