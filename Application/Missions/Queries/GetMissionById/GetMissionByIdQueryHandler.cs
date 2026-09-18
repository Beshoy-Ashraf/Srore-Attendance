using Application.Missions.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Queries.GetMissionById;

public class GetMissionByIdQueryHandler : IRequestHandler<GetMissionByIdQuery, MissionDto>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetMissionByIdQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<MissionDto> Handle(GetMissionByIdQuery request, CancellationToken cancellationToken)
      {
            var mission = await _unitOfWork.MissionRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Mission), request.Id);

            return new MissionDto(
                mission.Id,
                mission.StaffId,
                mission.Reason,
                mission.DateFrom,
                mission.DateTo,
                mission.Status,
                mission.ApprovedByAreaManagerId,
                mission.ApprovedDate);
      }
}