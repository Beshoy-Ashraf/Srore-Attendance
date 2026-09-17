using Application.Stores.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Stores.Queries.GetStoreById;

public class GetStoreByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStoreByIdQuery, StoreDto>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task<StoreDto> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
      {
            var store = await _unitOfWork.StoreRepository.GetByIdWithRoutersAsync(request.Id)
                ?? throw new NotFoundException(nameof(Store), request.Id);

            return new StoreDto(
                store.Id,
                store.Name,
                store.RouterMacs.Select(r => r.MacAddress).ToList(),
                store.AreaManagerId);
      }
}