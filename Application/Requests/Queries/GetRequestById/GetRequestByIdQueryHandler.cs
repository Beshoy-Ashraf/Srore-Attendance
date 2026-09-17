using Application.Requests.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Queries.GetRequestById;

public class GetRequestByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRequestByIdQuery, RequestDto>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;

      public async Task<RequestDto> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
      {
            var entity = await _unitOfWork.RequestRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Request), request.Id);

            return new RequestDto(
                entity.Id,
                entity.StaffId,
                entity.Type,
                entity.DateFrom,
                entity.DateTo,
                entity.Reason,
                entity.Status,
                entity.RequestedById,
                entity.ApprovedByAreaManagerId,
                entity.ApprovedDate);
      }
}