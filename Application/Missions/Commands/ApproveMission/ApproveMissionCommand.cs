using MediatR;

namespace Application.Missions.Commands.ApproveMission;

public record ApproveMissionCommand(Guid Id) : IRequest;
