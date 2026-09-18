namespace API.Contracts.Missions;

public record CreateMissionRequest(Guid StaffId, string Reason, DateOnly DateFrom, DateOnly DateTo);