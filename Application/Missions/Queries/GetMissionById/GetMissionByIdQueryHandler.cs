using Application.Common.Interfaces;
using Application.Missions.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Queries.GetMissionById;

public class GetMissionByIdQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetMissionByIdQuery, MissionDto>
{
      public async Task<MissionDto> Handle(GetMissionByIdQuery request, CancellationToken cancellationToken)
      {
            var mission = await unitOfWork.MissionRepository.GetDetailedByIdAsync(request.Id, includeDeleted: true, cancellationToken)
                ?? throw new NotFoundException(nameof(Mission), request.Id);

            await access.EnsureStaffAccessAsync(mission.StaffId, cancellationToken);

            return mission.ToDto();
      }
}
