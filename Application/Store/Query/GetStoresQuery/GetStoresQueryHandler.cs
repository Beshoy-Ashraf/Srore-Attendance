using Application.Stores.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Queries.GetStores;

public class GetStoresQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStoresQuery, IEnumerable<StoreDto>>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task<IEnumerable<StoreDto>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
      {
            var stores = await _unitOfWork.StoreRepository.GetFilteredAsync(
                request.AreaManagerId, request.Page, request.PageSize);

            return stores.Select(store => new StoreDto(
                store.Id,
                store.Name,
                store.RouterMacs.Select(r => r.MacAddress).ToList(),
                store.AreaManagerId));
      }
}