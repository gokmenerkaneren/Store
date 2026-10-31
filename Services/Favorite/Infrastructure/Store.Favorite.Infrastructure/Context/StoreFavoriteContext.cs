using Microsoft.EntityFrameworkCore;
using Store.Favorite.Domain.Entities;

namespace Store.Favorite.Infrastructure.Context
{
    public class StoreFavoriteContext : DbContext
    {
        public StoreFavoriteContext(DbContextOptions<StoreFavoriteContext> options) : base(options)
        {
        }
        public DbSet<FavoriteItem> FavoriteItems { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Store.Shared.Entities.BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.InsertDate = Store.Shared.Entities.BaseEntity.GetLocalTime();
                        entry.Entity.IsDeleted = false;
                        entry.Entity.IsActive = true;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdateDate = Store.Shared.Entities.BaseEntity.GetLocalTime();
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeleteDate = Store.Shared.Entities.BaseEntity.GetLocalTime();
                        entry.Entity.IsActive = false;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavoriteItem>().HasQueryFilter(x => !x.IsDeleted);
            base.OnModelCreating(modelBuilder);
        }
    }
}
