using Domain.Enums;

namespace API.Contracts.Requests;

public record CreateRequestRequest(
    Guid StaffId,
    RequestType Type,
    DateOnly DateFrom,
    DateOnly DateTo,
    string Reason);
public record RejectRequestRequest(string Reason);
