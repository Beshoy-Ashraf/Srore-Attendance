namespace Application.Schedules.Dtos;

/// <summary>One store's month that still has unapproved schedules, close enough to (or already inside)
/// that month that the Area Manager needs to approve or reject them now.</summary>
public record ScheduleApprovalAlertDto(
    Guid StoreId,
    string StoreName,
    int Year,
    int Month,
    int PendingCount,
    bool MonthHasStarted);
