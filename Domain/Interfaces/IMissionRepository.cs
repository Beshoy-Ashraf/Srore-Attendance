using Domain.Entities;

namespace Domain.Interfaces;

public interface IMissionRepository : IBaseRepository<Mission>
{
      Task<IEnumerable<Mission>> GetPendingByAreaManagerAsync(Guid areaManagerId);
      Task<IEnumerable<Mission>> GetByStaffAsync(Guid staffId);
}