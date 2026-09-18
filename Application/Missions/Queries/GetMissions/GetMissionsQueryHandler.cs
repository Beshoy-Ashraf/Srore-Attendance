using Application.Missions.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Queries.GetMissions;

public class GetMissionsQueryHandler : IRequestHandler<GetMissionsQuery, IEnumerable<MissionDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetMissionsQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<MissionDto>> Handle(GetMissionsQuery request, CancellationToken cancellationToken)
      {
            var missions = await _unitOfWork.MissionRepository.GetFilteredAsync(
                request.StaffId, request.Status, request.Page, request.PageSize);

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