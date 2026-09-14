using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.RejectSchedule;

public class RejectScheduleCommandHandler : IRequestHandler<RejectScheduleCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public RejectScheduleCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(RejectScheduleCommand request, CancellationToken cancellationToken)
      {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            if (schedule.Status != ScheduleStatus.Pending)
                  throw new BadRequestException("Only pending schedules can be rejected.");

            schedule.Status = ScheduleStatus.Rejected;
            schedule.ApprovedByAreaManagerId = request.ApprovedByAreaManagerId;
            schedule.ApprovedDate = DateTime.UtcNow;
            schedule.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.ScheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}