using Application.Missions.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Queries.GetPendingMissions;

public class GetPendingMissionsQueryHandler : IRequestHandler<GetPendingMissionsQuery, IEnumerable<MissionDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetPendingMissionsQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<MissionDto>> Handle(GetPendingMissionsQuery request, CancellationToken cancellationToken)
      {
            var missions = await _unitOfWork.MissionRepository.GetPendingByAreaManagerAsync(request.AreaManagerId);

            return missions.Select(m => new MissionDto(
                m.Id,
                m.StaffId,
                m.Reason,
                m.DateFrom,
                m.DateTo,
                m.Status,
                m.ApprovedByAreaManagerId,
                m.ApprovedDate));
      }
}