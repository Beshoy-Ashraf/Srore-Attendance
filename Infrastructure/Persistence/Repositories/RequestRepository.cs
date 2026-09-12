using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RequestRepository(AppDbContext context) : BaseRepository<Request>(context), IRequestRepository
{

      private readonly AppDbContext _context = context;
      public async Task<IEnumerable<Request>> GetPendingByAreaManagerAsync(Guid areaManagerId) =>
            await _context.Set<Request>()
                .Include(r => r.Staff)
                .Where(r => r.Status == RequestStatus.Pending && r.Staff.StoreId != null
                            && r.Staff.Store!.AreaManagerId == areaManagerId)
                .ToListAsync();

      public async Task<IEnumerable<Request>> GetByStaffAsync(Guid staffId) =>
          await _context.Set<Request>()
              .Where(r => r.StaffId == staffId)
              .OrderByDescending(r => r.CreatedDate)
              .ToListAsync();
}