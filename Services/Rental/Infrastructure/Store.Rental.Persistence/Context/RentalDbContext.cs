using Microsoft.EntityFrameworkCore;
using Store.Rental.Domain.Entities;

using Store.Rental.Application.Interfaces;

namespace Store.Rental.Persistence.Context
{
    public class RentalDbContext : DbContext, IRentalDbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options)
        {
        }

        public DbSet<RentalOrder> RentalOrders { get; set; }
        public DbSet<RentalOrderItem> RentalOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // DBA Kuralı: Soft Delete için Global Query Filter
            modelBuilder.Entity<RentalOrder>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<RentalOrderItem>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
