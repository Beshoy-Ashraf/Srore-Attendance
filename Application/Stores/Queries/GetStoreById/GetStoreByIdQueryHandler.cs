using Application.Common.Interfaces;
using Application.Stores.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Queries.GetStoreById;

public class GetStoreByIdQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetStoreByIdQuery, StoreDto>
{
    public async Task<StoreDto> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
    {
        await access.EnsureStoreAccessAsync(request.Id, cancellationToken);

        var store = await unitOfWork.StoreRepository.GetByIdWithDevicesAsync(request.Id)
            ?? throw new NotFoundException(nameof(Store), request.Id);

        return new StoreDto(
            store.Id,
            store.Name,
            store.Devices.Select(r => r.MacAddress).ToList(),
            store.AreaManagerId,
            store.AreaManager?.DisplayName);
    }
}
