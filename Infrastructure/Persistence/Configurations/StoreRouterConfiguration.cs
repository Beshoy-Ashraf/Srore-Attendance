using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class StoreRouterConfiguration : IEntityTypeConfiguration<StoreRouter>
{
      public void Configure(EntityTypeBuilder<StoreRouter> builder)
      {
            builder.ToTable("StoreRouters");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.MacAddress).IsRequired().HasMaxLength(17);
            builder.Property(r => r.Label).HasMaxLength(100);

            builder.HasIndex(r => r.MacAddress).IsUnique();
            builder.HasQueryFilter(r => r.Store == null || r.Store.DeleteDate == null);
      }
}