using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
      private readonly AppDbContext _dbContext;

      public IBaseRepository<User> Users { get; private set; } = null!;
      public IUserRepository UserRepository { get; private set; }

      public IStoreRepository StoreRepository { get; private set; } = null!;
      public IScheduleRepository ScheduleRepository { get; private set; } = null!;
      public IAttendanceRepository AttendanceRepository { get; private set; } = null!;
      public IRequestRepository RequestRepository { get; private set; } = null!;
      public IMissionRepository MissionRepository { get; private set; } = null!;
      public IDeviceRepository DeviceRepository { get; private set; } = null!;

      public UnitOfWork(AppDbContext dBContext)
      {
            _dbContext = dBContext;
            Users = new BaseRepository<User>(_dbContext);
            UserRepository = new UserRepository(_dbContext);
            StoreRepository = new StoreRepository(_dbContext);
            ScheduleRepository = new ScheduleRepository(_dbContext);
            AttendanceRepository = new AttendanceRepository(_dbContext);
            RequestRepository = new RequestRepository(_dbContext);
            MissionRepository = new MissionRepository(_dbContext);
            DeviceRepository = new DeviceRepository(_dbContext);

      }
      public async Task<int> Complete(CancellationToken cancellationToken)
      {
            return await _dbContext.SaveChangesAsync(cancellationToken);
      }

      public void Dispose()
      {
            _dbContext.Dispose();
      }
}
