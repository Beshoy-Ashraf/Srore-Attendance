using Application.Requests.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Queries.GetRequests;

public class GetRequestsQueryHandler : IRequestHandler<GetRequestsQuery, IEnumerable<RequestDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetRequestsQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<RequestDto>> Handle(GetRequestsQuery request, CancellationToken cancellationToken)
      {
            var requests = await _unitOfWork.RequestRepository.GetFilteredAsync(
                request.StaffId, request.Type, request.Status, request.Page, request.PageSize);

            return requests.Select(r => new RequestDto(
                r.Id,
                r.StaffId,
                r.Type,
                r.DateFrom,
                r.DateTo,
                r.Reason,
                r.Status,
                r.RequestedById,
                r.ApprovedByAreaManagerId,
                r.ApprovedDate));
      }
}