namespace API.Contracts.Missions;

public record CreateMissionRequest(Guid StaffId, string Reason, DateOnly DateFrom, DateOnly DateTo);
public record RejectMissionRequest(string Reason);
