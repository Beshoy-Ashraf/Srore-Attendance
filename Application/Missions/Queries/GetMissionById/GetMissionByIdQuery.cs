using Application.Missions.Dtos;
using MediatR;

namespace Application.Missions.Queries.GetMissionById;

public record GetMissionByIdQuery(Guid Id) : IRequest<MissionDto>;