using Application.Missions.Dtos;
using MediatR;

namespace Application.Missions.Queries.GetPendingMissions;

public record GetPendingMissionsQuery(Guid AreaManagerId) : IRequest<IEnumerable<MissionDto>>;