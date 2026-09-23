using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
      private readonly AppDbContext _context = context;

      public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken)
      {
            var normalized = email.Trim().ToLower();
            return await _context.Users
                  .Include(u => u.RefreshTokens)
                  .FirstOrDefaultAsync(u => u.DeleteDate == null && u.Email.ToLower() == normalized, cancellationToken);
      }

      public async Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken)
      {
            var normalized = username.Trim().ToLower();
            return await _context.Users
                  .FirstOrDefaultAsync(u => u.DeleteDate == null && u.Username.ToLower() == normalized, cancellationToken);
      }

      public async Task<User?> GetUserByUserId(Guid id, CancellationToken cancellationToken)
      {
            return await _context.Users
                  .Include(u => u.RefreshTokens)
                  .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
      }

      public async Task<User?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken)
      {
            return await _context.Users
                  .FirstOrDefaultAsync(u => u.Id == id && u.DeleteDate == null, cancellationToken);
      }

      public async Task<PagedList<User>> GetPagedAsync(UserFilter filter, CancellationToken cancellationToken)
      {
            IQueryable<User> query = _context.Users.Include(u => u.Store).Where(u => u.DeleteDate == null);

            if (filter.OnlyUserId.HasValue)
                  query = query.Where(u => u.Id == filter.OnlyUserId.Value);

            if (filter.Role.HasValue)
                  query = query.Where(u => u.Role == filter.Role.Value);

            if (filter.Roles is { Count: > 0 })
            {
                  var roles = filter.Roles.ToList();
                  query = query.Where(u => roles.Contains(u.Role));
            }

            if (filter.StoreIds is not null)
            {
                  var storeIds = filter.StoreIds.ToList();
                  var also = filter.AlsoIncludeUserIds?.ToList() ?? new List<Guid>();
                  query = query.Where(u => (u.StoreId != null && storeIds.Contains(u.StoreId.Value)) || also.Contains(u.Id));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                  var term = filter.Search.Trim().ToLower();
                  query = query.Where(u => u.DisplayName.ToLower().Contains(term)
                                        || u.Username.ToLower().Contains(term)
                                        || u.Email.ToLower().Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                  .OrderBy(u => u.DisplayName)
                  .ThenBy(u => u.Id)
                  .Skip((filter.Page - 1) * filter.PageSize)
                  .Take(filter.PageSize)
                  .ToListAsync(cancellationToken);

            return new PagedList<User>(items, total);
      }
}
