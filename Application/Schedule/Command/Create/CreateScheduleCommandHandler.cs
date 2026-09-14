using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Guid>
{
      private readonly IUnitOfWork _unitOfWork;

      public CreateScheduleCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<Guid> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
      {
            var existing = await _unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(request.StaffId, request.Date);
            if (existing is not null)
                  throw new ConflictException("This staff member already has a schedule for that date.");

            var schedule = new Schedule
            {
                  Id = Guid.NewGuid(),
                  StaffId = request.StaffId,
                  Date = request.Date,
                  ShiftType = request.ShiftType,
                  StartTime = request.StartTime,
                  EndTime = request.EndTime,
                  Status = ScheduleStatus.Pending,
                  CreatedByStoreManagerId = request.CreatedByStoreManagerId,
                  CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.ScheduleRepository.AddAsync(schedule, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            return schedule.Id;
      }
}