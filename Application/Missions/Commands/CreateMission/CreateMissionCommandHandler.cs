using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Missions.Commands.CreateMission;

public class CreateMissionCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<CreateMissionCommand, Guid>
{
      public async Task<Guid> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureStaffAccessAsync(request.StaffId, cancellationToken);

            if (await unitOfWork.MissionRepository.HasOverlapAsync(
                    request.StaffId, request.DateFrom, request.DateTo, excludeId: null, cancellationToken))
                  throw new ConflictException("There's already a pending or approved mission covering that range.");

            var mission = new Mission
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  Reason = request.Reason,
                  DateFrom = request.DateFrom,
                  DateTo = request.DateTo,
                  Status = RequestStatus.Pending,
                  CreatedDate = clock.UtcNow
            };

            await unitOfWork.MissionRepository.AddAsync(mission, cancellationToken);
            await unitOfWork.Complete(cancellationToken);

            return mission.Id;
      }
}
