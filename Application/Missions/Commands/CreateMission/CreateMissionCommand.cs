using MediatR;

namespace Application.Missions.Commands.CreateMission;

public record CreateMissionCommand(
    Guid StaffId,
    string Reason,
    DateOnly DateFrom,
    DateOnly DateTo) : IRequest<Guid>;