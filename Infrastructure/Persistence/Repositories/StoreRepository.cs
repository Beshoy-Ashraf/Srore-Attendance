using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class StoreRepository(AppDbContext context) : BaseRepository<Store>(context), IStoreRepository
{

    private readonly AppDbContext _context = context;

    private static string NormalizeMac(string mac)
    {
        if (string.IsNullOrWhiteSpace(mac))
            return string.Empty;

        return mac.Trim().Replace("-", ":").ToUpperInvariant();
    }

    public async Task<Store?> GetByRouterMacAsync(string routerMac)
    {
        var normalizedRouterMac = NormalizeMac(routerMac);
        return await _context.Set<Store>()
            .Include(s => s.RouterMacs)
            .FirstOrDefaultAsync(s => s.RouterMacs.Any(r => r.MacAddress == normalizedRouterMac));
    }

    public async Task<bool> HasRouterMacAsync(Guid storeId, string routerMac)
    {
        var normalizedRouterMac = NormalizeMac(routerMac);
        return await _context.Set<StoreRouter>()
            .AnyAsync(r => r.StoreId == storeId && r.MacAddress == normalizedRouterMac);
    }

    public async Task<Store?> GetByIdWithRoutersAsync(Guid id) =>
        await _context.Set<Store>()
            .Include(s => s.RouterMacs)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Store>> GetByAreaManagerAsync(Guid areaManagerId) =>
        await _context.Set<Store>().Where(s => s.AreaManagerId == areaManagerId).ToListAsync();

    public async Task<IEnumerable<Store>> GetFilteredAsync(Guid? areaManagerId, int page, int pageSize)
    {
        var query = _context.Set<Store>().Include(s => s.RouterMacs).AsQueryable();

        if (areaManagerId.HasValue)
            query = query.Where(s => s.AreaManagerId == areaManagerId.Value);

        return await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}