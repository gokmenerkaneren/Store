using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Rental.Domain.Entities;

namespace Store.Rental.Application.Interfaces
{
    public interface IRentalDbContext
    {
        DbSet<RentalOrder> RentalOrders { get; set; }
        DbSet<RentalOrderItem> RentalOrderItems { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
