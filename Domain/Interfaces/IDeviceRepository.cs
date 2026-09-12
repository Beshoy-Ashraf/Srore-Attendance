using Domain.Entities;

namespace Domain.Interfaces;

public interface IDeviceRepository : IBaseRepository<Device>
{
      Task<Device?> GetByStaffIdAsync(Guid staffId);
}