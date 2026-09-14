using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.UpdateSchedule;

public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public UpdateScheduleCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
      {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            schedule.ShiftType = request.ShiftType;
            schedule.StartTime = request.StartTime;
            schedule.EndTime = request.EndTime;
            schedule.UpdateDate = DateTime.UtcNow;

            // Any change to an already-decided schedule needs to go back through approval
            schedule.Status = ScheduleStatus.Pending;
            schedule.ApprovedByAreaManagerId = null;
            schedule.ApprovedDate = null;

            await _unitOfWork.ScheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}