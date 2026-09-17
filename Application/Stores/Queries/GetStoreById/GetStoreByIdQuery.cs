using Application.Stores.Dtos;
using MediatR;

namespace Application.Stores.Queries.GetStoreById;

public record GetStoreByIdQuery(Guid Id) : IRequest<StoreDto>;