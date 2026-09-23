using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

/// <summary>
/// Optional start-up tasks, both opt-in through configuration:
/// <c>Database:AutoMigrate</c> applies pending EF migrations, and <c>Seed:AdminEmail</c> /
/// <c>Seed:AdminPassword</c> create the first Admin when the Users table is empty
/// (self-registration is disabled, so a fresh install needs one).
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var configuration = provider.GetRequiredService<IConfiguration>();
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");
        var db = provider.GetRequiredService<AppDbContext>();

        if (bool.TryParse(configuration["Database:AutoMigrate"], out var autoMigrate) && autoMigrate)
        {
            logger.LogInformation("Applying pending database migrations...");
            await db.Database.MigrateAsync(cancellationToken);
        }

        var adminEmail = configuration["Seed:AdminEmail"];
        var adminPassword = configuration["Seed:AdminPassword"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            return;

        try
        {
            if (await db.Set<User>().AnyAsync(cancellationToken))
                return;

            var hasher = provider.GetRequiredService<IPasswordHasher>();
            var username = configuration["Seed:AdminUsername"];
            if (string.IsNullOrWhiteSpace(username))
                username = "admin";

            var displayName = configuration["Seed:AdminDisplayName"];
            if (string.IsNullOrWhiteSpace(displayName))
                displayName = "Administrator";

            var admin = new User(username.Trim(), adminEmail.Trim(), hasher.Hash(adminPassword), displayName.Trim(), string.Empty, UserRole.Admin)
            {
                CreatedDate = DateTime.UtcNow
            };

            db.Set<User>().Add(admin);
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded the first admin account '{Username}'. Change its password after signing in.", username);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not seed the admin account (has the database been migrated?).");
        }
    }
}
