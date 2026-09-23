using Application.Common.Models;
using Application.Missions.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Missions.Queries.GetMissions;

public record GetMissionsQuery(
    Guid? StaffId,
    Guid? StoreId,
    RequestStatus? Status,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<MissionDto>>;
