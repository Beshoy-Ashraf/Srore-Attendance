using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface IRequestRepository : IBaseRepository<Request>
{
      Task<IEnumerable<Request>> GetPendingByAreaManagerAsync(Guid areaManagerId);
      Task<IEnumerable<Request>> GetByStaffAsync(Guid staffId);

      Task<IEnumerable<Request>> GetFilteredAsync(
          Guid? staffId, RequestType? type, RequestStatus? status, int page, int pageSize);
}