using Application.Common.Models;
using Application.Missions.Dtos;
using MediatR;

namespace Application.Missions.Queries.GetPendingMissions;

public record GetPendingMissionsQuery(
    Guid? StoreId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<MissionDto>>;
