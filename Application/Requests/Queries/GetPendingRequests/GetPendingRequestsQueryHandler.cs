using Application.Requests.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Queries.GetPendingRequests;

public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, IEnumerable<RequestDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetPendingRequestsQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<RequestDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
      {
            var requests = await _unitOfWork.RequestRepository.GetPendingByAreaManagerAsync(request.AreaManagerId);

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