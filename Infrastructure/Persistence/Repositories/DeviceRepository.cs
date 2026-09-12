using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DeviceRepository(AppDbContext context) : BaseRepository<Device>(context), IDeviceRepository
{
      private readonly AppDbContext _context = context;

      public async Task<Device?> GetByStaffIdAsync(Guid staffId) =>
          await _context.Set<Device>().FirstOrDefaultAsync(d => d.StaffId == staffId);
}