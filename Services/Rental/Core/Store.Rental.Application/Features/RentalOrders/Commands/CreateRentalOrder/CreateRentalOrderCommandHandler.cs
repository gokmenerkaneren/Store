using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Store.Rental.Domain.Entities;
using Store.Rental.Domain.Enums;
using Store.Rental.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Store.Rental.Application.Features.RentalOrders.Commands.CreateRentalOrder
{
    public class CreateRentalOrderCommandHandler : IRequestHandler<CreateRentalOrderCommand, Guid>
    {
        private readonly IRentalDbContext _context;

        public CreateRentalOrderCommandHandler(IRentalDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateRentalOrderCommand request, CancellationToken cancellationToken)
        {
            // Business Rule: Tarih Çakışması Kontrolü (Conflict Check)
            var productIds = request.Items.Select(x => x.ProductId).ToList();
            
            var conflictExists = await _context.RentalOrderItems
                .Include(i => i.RentalOrder)
                .Where(i => productIds.Contains(i.ProductId))
                .Where(i => i.RentalOrder.Status == RentalStatus.Approved || i.RentalOrder.Status == RentalStatus.Delivered)
                .AnyAsync(i => 
                    request.StartDate <= i.RentalOrder.EndDate && 
                    request.EndDate >= i.RentalOrder.StartDate, 
                    cancellationToken);

            if (conflictExists)
            {
                // TODO: Custom ApiException fırlatılacak
                throw new Exception("BusinessRuleException: Seçili tarihler arasında stok çakışması tespit edildi.");
            }

            var rentalOrder = new RentalOrder
            {
                UserId = request.UserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = RentalStatus.Pending,
                TotalAmount = request.Items.Sum(x => x.Quantity * x.UnitPrice),
                InsertUserId = request.UserId
            };

            foreach (var item in request.Items)
            {
                rentalOrder.RentalOrderItems.Add(new RentalOrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    InsertUserId = request.UserId
                });
            }

            _context.RentalOrders.Add(rentalOrder);
            await _context.SaveChangesAsync(cancellationToken);

            return rentalOrder.Id;
        }
    }
}
