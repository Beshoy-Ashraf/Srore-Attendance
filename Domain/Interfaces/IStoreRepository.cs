using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces;

public interface IStoreRepository : IBaseRepository<Store>
{


    Task<bool> HasDeviceMacAsync(Guid storeId, string deviceMac);

    /// <summary>True if this device MAC is already registered to any store (device MACs are globally unique).</summary>
    Task<bool> IsDeviceMacRegisteredAsync(string deviceMac, Guid? excludeStoreId = null);

    Task<Store?> GetByIdWithDevicesAsync(Guid id);

    /// <summary>Stores (tracked) managed by the given area manager.</summary>
    Task<IReadOnlyList<Store>> GetByAreaManagerAsync(Guid areaManagerId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> GetIdsByAreaManagerAsync(Guid areaManagerId, CancellationToken cancellationToken);

    Task<PagedList<Store>> GetPagedAsync(
        IReadOnlyCollection<Guid>? storeIds, Guid? areaManagerId, int page, int pageSize, CancellationToken cancellationToken);

    Task<bool> HasActiveStaffAsync(Guid storeId, CancellationToken cancellationToken);
}