using Domain.Enums;

namespace Application.Requests.Dtos;

public record RequestDto(
    Guid Id,
    Guid StaffId,
    RequestType Type,
    DateOnly DateFrom,
    DateOnly DateTo,
    string Reason,
    RequestStatus Status,
    Guid RequestedById,
    Guid? ApprovedByAreaManagerId,
    DateTime? ApprovedDate);