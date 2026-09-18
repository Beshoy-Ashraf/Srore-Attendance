using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Commands.CreateMission;

public class CreateMissionCommandHandler : IRequestHandler<CreateMissionCommand, Guid>
{
      private readonly IUnitOfWork _unitOfWork;

      public CreateMissionCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<Guid> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
      {
            var mission = new Mission
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  Reason = request.Reason,
                  DateFrom = request.DateFrom,
                  DateTo = request.DateTo,
                  Status = RequestStatus.Pending,
                  CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.MissionRepository.AddAsync(mission, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            return mission.Id;
      }
}