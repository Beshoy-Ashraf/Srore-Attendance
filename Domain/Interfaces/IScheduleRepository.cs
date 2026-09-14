using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface IScheduleRepository : IBaseRepository<Schedule>
{
      Task<Schedule?> GetByStaffAndDateAsync(Guid staffId, DateOnly date);
      Task<IEnumerable<Schedule>> GetByStaffAndDateRangeAsync(Guid staffId, DateOnly from, DateOnly to);
      Task<IEnumerable<Schedule>> GetPendingByAreaManagerAsync(Guid areaManagerId);


      Task<IEnumerable<Schedule>> GetFilteredAsync(
          Guid? staffId, Guid? storeId, DateOnly? from, DateOnly? to, ScheduleStatus? status, int page, int pageSize);
}