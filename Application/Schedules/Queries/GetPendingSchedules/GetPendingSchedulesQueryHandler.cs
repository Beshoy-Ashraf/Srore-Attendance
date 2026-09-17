using Application.Schedules.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Queries.GetPendingSchedules;

public class GetPendingSchedulesQueryHandler : IRequestHandler<GetPendingSchedulesQuery, IEnumerable<ScheduleDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetPendingSchedulesQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<ScheduleDto>> Handle(GetPendingSchedulesQuery request, CancellationToken cancellationToken)
      {
            var schedules = await _unitOfWork.ScheduleRepository.GetPendingByAreaManagerAsync(request.AreaManagerId);

            return schedules.Select(s => new ScheduleDto(
                s.Id,
                s.StaffId,
                s.Date,
                s.ShiftType,
                s.StartTime,
                s.EndTime,
                s.Status,
                s.CreatedByStoreManagerId,
                s.ApprovedByAreaManagerId,
                s.ApprovedDate));
      }
}