using Domain.Enums;

namespace Application.Missions.Dtos;

public record MissionDto(
    Guid Id,
    Guid StaffId,
    string? StaffName,
    string Reason,
    DateOnly DateFrom,
    DateOnly DateTo,
    RequestStatus Status,
    Guid? ApprovedByAreaManagerId,
    string? ApprovedByAreaManagerName,
    DateTime? ApprovedDate,
    string? RejectionReason);

public static class MissionMappings
{
    public static MissionDto ToDto(this Domain.Entities.Mission m) => new(
        m.Id,
        m.StaffId,
        m.Staff?.DisplayName,
        m.Reason,
        m.DateFrom,
        m.DateTo,
        m.Status,
        m.ApprovedByAreaManagerId,
        m.ApprovedByAreaManager?.DisplayName,
        m.ApprovedDate,
        m.RejectionReason);
}
