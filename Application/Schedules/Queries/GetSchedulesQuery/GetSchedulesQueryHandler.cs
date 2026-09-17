using Application.Schedules.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Queries.GetSchedules;

public class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQuery, IEnumerable<ScheduleDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetSchedulesQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<ScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
      {
            var schedules = await _unitOfWork.ScheduleRepository.GetFilteredAsync(
                request.StaffId, request.StoreId, request.From, request.To, request.Status, request.Page, request.PageSize);

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