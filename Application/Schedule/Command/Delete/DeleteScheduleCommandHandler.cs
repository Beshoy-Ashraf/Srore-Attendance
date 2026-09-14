using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Commands.DeleteSchedule;

public class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public DeleteScheduleCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
      {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            schedule.DeletedDate = DateTime.UtcNow;
            await _unitOfWork.ScheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}