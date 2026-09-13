using Domain.Entities;

namespace Domain.Interfaces;

public interface IStoreRepository : IBaseRepository<Store>
{
      Task<Store?> GetByRouterMacAsync(string routerMac);

      Task<bool> HasRouterMacAsync(Guid storeId, string routerMac);

      Task<Store?> GetByIdWithRoutersAsync(Guid id);

      Task<IEnumerable<Store>> GetByAreaManagerAsync(Guid areaManagerId);
      Task<IEnumerable<Store>> GetFilteredAsync(Guid? areaManagerId, int page, int pageSize);
}