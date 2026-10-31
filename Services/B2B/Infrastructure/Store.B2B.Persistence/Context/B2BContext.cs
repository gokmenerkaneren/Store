using Microsoft.EntityFrameworkCore;
using Store.B2B.Domain.Entities;

namespace Store.B2B.Persistence.Context
{
    public class B2BContext : DbContext
    {
        public B2BContext(DbContextOptions<B2BContext> options) : base(options)
        {
        }

        public DbSet<DealerFinancial> DealerFinancials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<DealerFinancial>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TaxNumber).IsUnique(); // TaxNumber must be unique
                entity.Property(e => e.DiscountRate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}
