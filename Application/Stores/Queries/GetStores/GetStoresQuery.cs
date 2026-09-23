using Application.Common.Models;
using Application.Stores.Dtos;
using MediatR;

namespace Application.Stores.Queries.GetStores;

public record GetStoresQuery(
    Guid? AreaManagerId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<StoreDto>>;
