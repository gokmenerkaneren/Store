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
            // 1. Tarih doğrulaması ve kiralama süresi hesabı (K7 Fiyat ve Süre Koruması)
            if (request.EndDate <= request.StartDate)
            {
                throw new InvalidOperationException("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");
            }

            var rentalDays = (int)Math.Ceiling((request.EndDate.Date - request.StartDate.Date).TotalDays);
            if (rentalDays <= 0)
            {
                rentalDays = 1;
            }

            // 2. Kalem ve Fiyat Doğrulaması (K7 Fiyat Manipülasyon Engeli)
            if (request.Items == null || !request.Items.Any())
            {
                throw new InvalidOperationException("Kiralama siparişinde en az bir ürün bulunmalıdır.");
            }

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException("Ürün miktarı 0'dan büyük olmalıdır.");
                }

                if (item.UnitPrice <= 0)
                {
                    throw new InvalidOperationException("Ürün birim fiyatı 0'dan büyük olmalıdır.");
                }
            }

            // 3. Tarih Çakışması Kontrolü (Conflict Check)
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
                throw new InvalidOperationException("Seçili tarihler arasında stok çakışması tespit edildi.");
            }

            // 4. Sunucu Taraflı Toplam Fiyat Hesabı (Miktar * Birim Fiyat * Gün Sayısı)
            var totalCalculatedAmount = request.Items.Sum(x => x.Quantity * x.UnitPrice * rentalDays);

            var rentalOrder = new RentalOrder
            {
                UserId = request.UserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = RentalStatus.Pending,
                TotalAmount = totalCalculatedAmount,
                InsertUserId = request.UserId
            };

            foreach (var item in request.Items)
            {
                rentalOrder.RentalOrderItems.Add(new RentalOrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    InsertUserId = request.UserId
                });
            }

            _context.RentalOrders.Add(rentalOrder);
            await _context.SaveChangesAsync(cancellationToken);

            return rentalOrder.Id;
        }
    }
}
