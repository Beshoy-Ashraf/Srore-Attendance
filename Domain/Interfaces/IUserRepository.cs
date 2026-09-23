using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>Case-insensitive lookup among active (not deleted) users. Includes refresh tokens.</summary>
    Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken);

    /// <summary>Case-insensitive lookup among active (not deleted) users.</summary>
    Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken);

    /// <summary>Loads a user (deleted or not) together with their refresh tokens.</summary>
    Task<User?> GetUserByUserId(Guid id, CancellationToken cancellationToken);

    /// <summary>Loads an active (not deleted) user without any includes.</summary>
    Task<User?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedList<User>> GetPagedAsync(UserFilter filter, CancellationToken cancellationToken);
}
