using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
      private readonly AppDbContext _context = context;

      public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken)
      {
            return await _context.Users
                  .Include(u => u.RefreshTokens)
                  .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
      }
      public async Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken)
      {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
      }
      public async Task<User?> GetUserByUserId(Guid id, CancellationToken cancellationToken)
      {
            return await _context.Users
                  .Include(u => u.RefreshTokens)
                  .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
      }
      public async Task<List<User>?> GetFilteredAsync(Guid? StoreId, Domain.Enums.UserRole? Role, int Page, int PageSize, CancellationToken cancellationToken)
      {
            var query = _context.Users.AsQueryable();

            if (StoreId.HasValue)
            {
                  query = query.Where(u => u.StoreId == StoreId.Value);
            }

            if (Role.HasValue)
            {
                  query = query.Where(u => u.Role == Role.Value);
            }

            return await query.Skip((Page - 1) * PageSize).Take(PageSize).ToListAsync(cancellationToken);
      }
}