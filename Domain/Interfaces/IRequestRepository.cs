using Domain.Entities;

namespace Domain.Interfaces;

public interface IRequestRepository : IBaseRepository<Request>
{
      Task<IEnumerable<Request>> GetPendingByAreaManagerAsync(Guid areaManagerId);
      Task<IEnumerable<Request>> GetByStaffAsync(Guid staffId);
}