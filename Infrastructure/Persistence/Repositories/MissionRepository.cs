using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MissionRepository(AppDbContext context) : BaseRepository<Mission>(context), IMissionRepository
{
      private readonly AppDbContext _context = context;

      public async Task<IEnumerable<Mission>> GetPendingByAreaManagerAsync(Guid areaManagerId) =>
            await _context.Set<Mission>()
                .Include(m => m.Staff)
                .Where(m => m.Status == RequestStatus.Pending && m.Staff.StoreId != null
                            && m.Staff.Store!.AreaManagerId == areaManagerId)
                .ToListAsync();

      public async Task<IEnumerable<Mission>> GetByStaffAsync(Guid staffId) =>
          await _context.Set<Mission>()
              .Where(m => m.StaffId == staffId)
              .OrderByDescending(m => m.CreatedDate)
              .ToListAsync();
}