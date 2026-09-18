using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AttendanceSettingsRepository(AppDbContext context) : BaseRepository<AttendanceSettings>(context), IAttendanceSettingsRepository
{
      private readonly AppDbContext _context = context;

      public async Task<AttendanceSettings?> GetByStoreIdAsync(Guid storeId) =>
            await _context.Set<AttendanceSettings>().FirstOrDefaultAsync(a => a.StoreId == storeId);
}