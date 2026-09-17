using Application.Requests.Dtos;
using MediatR;

namespace Application.Requests.Queries.GetRequestById;

public record GetRequestByIdQuery(Guid Id) : IRequest<RequestDto>;