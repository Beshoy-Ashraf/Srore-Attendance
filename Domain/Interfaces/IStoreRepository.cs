using Domain.Entities;

namespace Domain.Interfaces;

public interface IStoreRepository : IBaseRepository<Store>
{
      Task<Store?> GetByRouterMacAsync(string routerMac);
      Task<IEnumerable<Store>> GetByAreaManagerAsync(Guid areaManagerId);
}