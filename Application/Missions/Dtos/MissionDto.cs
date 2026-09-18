using Domain.Enums;

namespace Application.Missions.Dtos;

public record MissionDto(
    Guid Id,
    Guid StaffId,
    string Reason,
    DateOnly DateFrom,
    DateOnly DateTo,
    RequestStatus Status,
    Guid? ApprovedByAreaManagerId,
    DateTime? ApprovedDate);