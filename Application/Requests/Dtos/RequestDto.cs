using Domain.Enums;

namespace Application.Requests.Dtos;

public record RequestDto(
    Guid Id,
    Guid StaffId,
    string? StaffName,
    RequestType Type,
    DateOnly DateFrom,
    DateOnly DateTo,
    string Reason,
    RequestStatus Status,
    Guid RequestedById,
    Guid? ApprovedByAreaManagerId,
    string? ApprovedByAreaManagerName,
    DateTime? ApprovedDate,
    string? RejectionReason);

public static class RequestMappings
{
    public static RequestDto ToDto(this Domain.Entities.Request r) => new(
        r.Id,
        r.StaffId,
        r.Staff?.DisplayName,
        r.Type,
        r.DateFrom,
        r.DateTo,
        r.Reason,
        r.Status,
        r.RequestedById,
        r.ApprovedByAreaManagerId,
        r.ApprovedByAreaManager?.DisplayName,
        r.ApprovedDate,
        r.RejectionReason);
}
