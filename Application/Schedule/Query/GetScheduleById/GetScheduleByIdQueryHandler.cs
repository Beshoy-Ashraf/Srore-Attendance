using Application.Schedules.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Queries.GetScheduleById;

public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, ScheduleDto>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetScheduleByIdQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<ScheduleDto> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
      {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            return new ScheduleDto(
                schedule.Id,
                schedule.StaffId,
                schedule.Date,
                schedule.ShiftType,
                schedule.StartTime,
                schedule.EndTime,
                schedule.Status,
                schedule.CreatedByStoreManagerId,
                schedule.ApprovedByAreaManagerId,
                schedule.ApprovedDate);
      }
}