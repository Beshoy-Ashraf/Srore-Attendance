using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Stores.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Queries.GetStores;

public class GetStoresQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetStoresQuery, PagedResult<StoreDto>>
{
      public async Task<PagedResult<StoreDto>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
      {
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);
            var storeIds = await access.GetVisibleStoreIdsAsync(cancellationToken);

            var result = await unitOfWork.StoreRepository.GetPagedAsync(
                storeIds, request.AreaManagerId, page, pageSize, cancellationToken);

            return PagedResult<StoreDto>.From(result, ToDto, page, pageSize);
      }

      private static StoreDto ToDto(Domain.Entities.Store store) => new(
          store.Id,
          store.Name,
          store.Devices.Select(r => r.MacAddress).ToList(),
          store.AreaManagerId,
          store.AreaManager?.DisplayName);
}
