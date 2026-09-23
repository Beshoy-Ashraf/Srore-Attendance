using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces;

public interface IMissionRepository : IBaseRepository<Mission>
{
    Task<Mission?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Mission?> GetDetailedByIdAsync(Guid id, bool includeDeleted, CancellationToken cancellationToken);

    Task<PagedList<Mission>> GetPagedAsync(MissionFilter filter, CancellationToken cancellationToken);

    Task<IReadOnlyList<Mission>> GetApprovedOverlappingAsync(
        IReadOnlyCollection<Guid> staffIds, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    Task<bool> HasOverlapAsync(Guid staffId, DateOnly from, DateOnly to, Guid? excludeId, CancellationToken cancellationToken);
}
