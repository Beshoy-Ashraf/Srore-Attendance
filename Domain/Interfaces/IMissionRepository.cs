using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface IMissionRepository : IBaseRepository<Mission>
{
      Task<IEnumerable<Mission>> GetPendingByAreaManagerAsync(Guid areaManagerId);
      Task<IEnumerable<Mission>> GetByStaffAsync(Guid staffId);
      Task<IEnumerable<Mission>> GetFilteredAsync(Guid? staffId, RequestStatus? status, int page, int pageSize);
}