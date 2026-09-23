using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
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

    public async Task<Store?> GetByDeviceMacAsync(string deviceMac)
    {
        var normalizedDeviceMac = NormalizeMac(deviceMac);
        return await _context.Set<Store>()
            .Include(s => s.Devices)
            .FirstOrDefaultAsync(s => s.Devices.Any(r => r.MacAddress == normalizedDeviceMac));
    }


    public async Task<bool> HasDeviceMacAsync(Guid storeId, string deviceMac)
    {
        var normalizedDeviceMac = NormalizeMac(deviceMac);
        return await _context.Set<StoreDevice>()
            .AnyAsync(d => d.StoreId == storeId && d.MacAddress == normalizedDeviceMac);
    }

    public async Task<bool> IsDeviceMacRegisteredAsync(string deviceMac, Guid? excludeStoreId = null)
    {
        var normalizedDeviceMac = NormalizeMac(deviceMac);
        return await _context.Set<StoreDevice>()
            .AnyAsync(d => d.MacAddress == normalizedDeviceMac && (excludeStoreId == null || d.StoreId != excludeStoreId));
    }

    public async Task<Store?> GetByIdWithDevicesAsync(Guid id) =>
        await _context.Set<Store>()
            .Include(s => s.Devices)
            .Include(s => s.AreaManager)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IReadOnlyList<Store>> GetByAreaManagerAsync(Guid areaManagerId, CancellationToken cancellationToken) =>
        await _context.Set<Store>().Where(s => s.AreaManagerId == areaManagerId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetIdsByAreaManagerAsync(Guid areaManagerId, CancellationToken cancellationToken) =>
        await _context.Set<Store>()
            .Where(s => s.AreaManagerId == areaManagerId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

    public async Task<PagedList<Store>> GetPagedAsync(
        IReadOnlyCollection<Guid>? storeIds, Guid? areaManagerId, int page, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<Store> query = _context.Set<Store>()
            .Include(s => s.Devices)
            .Include(s => s.AreaManager);

        if (storeIds is not null)
        {
            var ids = storeIds.ToList();
            query = query.Where(s => ids.Contains(s.Id));
        }

        if (areaManagerId.HasValue)
            query = query.Where(s => s.AreaManagerId == areaManagerId.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(s => s.Name)
            .ThenBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Store>(items, total);
    }

    public async Task<bool> HasActiveStaffAsync(Guid storeId, CancellationToken cancellationToken) =>
        await _context.Set<User>().AnyAsync(u => u.StoreId == storeId && u.DeleteDate == null, cancellationToken);
}