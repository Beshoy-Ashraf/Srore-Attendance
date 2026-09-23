using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AttendanceRepository(AppDbContext context) : BaseRepository<Attendance>(context), IAttendanceRepository
{
      private readonly AppDbContext _context = context;

      public async Task<Attendance?> GetOpenAttendanceAsync(Guid staffId) =>
            await _context.Set<Attendance>()
                .Where(a => a.StaffId == staffId && a.CheckInTime != null && a.CheckOutTime == null)
                .OrderByDescending(a => a.CheckInTime)
                .FirstOrDefaultAsync();

      public async Task<Attendance?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken) =>
            await _context.Set<Attendance>()
                .Include(a => a.Staff).ThenInclude(u => u.Store)
                .Include(a => a.EnteredManuallyByUser)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

      public async Task<PagedList<Attendance>> GetPagedAsync(AttendanceFilter filter, CancellationToken cancellationToken)
      {
            IQueryable<Attendance> query = _context.Set<Attendance>()
                .Include(a => a.Staff).ThenInclude(u => u.Store)
                .Include(a => a.EnteredManuallyByUser);

            if (filter.StaffId.HasValue)
                  query = query.Where(a => a.StaffId == filter.StaffId.Value);

            if (filter.StoreIds is not null)
            {
                  var storeIds = filter.StoreIds.ToList();
                  query = query.Where(a => a.Staff.StoreId != null && storeIds.Contains(a.Staff.StoreId.Value));
            }

            if (filter.FromUtc.HasValue)
                  query = query.Where(a => a.CheckInTime >= filter.FromUtc.Value);

            if (filter.ToUtcExclusive.HasValue)
                  query = query.Where(a => a.CheckInTime < filter.ToUtcExclusive.Value);

            if (filter.LateOnly == true)
                  query = query.Where(a => a.IsLate);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(a => a.CheckInTime)
                .ThenBy(a => a.Id)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<Attendance>(items, total);
      }

      public async Task<IReadOnlyList<Attendance>> GetInRangeAsync(
            IReadOnlyCollection<Guid> staffIds, DateTime fromUtc, DateTime toUtcExclusive, CancellationToken cancellationToken)
      {
            var ids = staffIds.ToList();
            return await _context.Set<Attendance>()
                .Where(a => ids.Contains(a.StaffId)
                            && a.CheckInTime != null
                            && a.CheckInTime >= fromUtc
                            && a.CheckInTime < toUtcExclusive)
                .OrderBy(a => a.CheckInTime)
                .ToListAsync(cancellationToken);
      }
}
