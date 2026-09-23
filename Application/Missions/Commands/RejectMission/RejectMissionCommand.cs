using MediatR;

namespace Application.Missions.Commands.RejectMission;

public record RejectMissionCommand(Guid Id, string Reason) : IRequest;
