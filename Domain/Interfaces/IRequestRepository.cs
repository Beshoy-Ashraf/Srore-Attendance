using Domain.Entities;
using Domain.Enums;
using Domain.Models;

namespace Domain.Interfaces;

public interface IRequestRepository : IBaseRepository<Request>
{
    Task<Request?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Request?> GetDetailedByIdAsync(Guid id, bool includeDeleted, CancellationToken cancellationToken);

    Task<PagedList<Request>> GetPagedAsync(RequestFilter filter, CancellationToken cancellationToken);

    /// <summary>True when the staff member already has a pending/approved request of this type touching the range.</summary>
    Task<bool> HasOverlapAsync(
        Guid staffId, RequestType type, DateOnly from, DateOnly to, Guid? excludeId, CancellationToken cancellationToken);
}
