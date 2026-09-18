using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Commands.RejectMission;

public class RejectMissionCommandHandler : IRequestHandler<RejectMissionCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public RejectMissionCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(RejectMissionCommand request, CancellationToken cancellationToken)
      {
            var mission = await _unitOfWork.MissionRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Mission), request.Id);

            if (mission.Status != RequestStatus.Pending)
                  throw new BadRequestException("Only pending missions can be rejected.");

            mission.Status = RequestStatus.Rejected;
            mission.ApprovedByAreaManagerId = request.ApprovedByAreaManagerId;
            mission.ApprovedDate = DateTime.UtcNow;
            mission.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.MissionRepository.UpdateAsync(mission, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}