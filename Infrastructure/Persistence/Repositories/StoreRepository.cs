using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class StoreRepository(AppDbContext context) : BaseRepository<Store>(context), IStoreRepository
{
      private readonly AppDbContext _context = context;

      public async Task<Store?> GetByRouterMacAsync(string routerMac) =>
            await _context.Set<Store>().FirstOrDefaultAsync(s => s.RouterMac == routerMac);

      public async Task<IEnumerable<Store>> GetByAreaManagerAsync(Guid areaManagerId) =>
          await _context.Set<Store>().Where(s => s.AreaManagerId == areaManagerId).ToListAsync();
}