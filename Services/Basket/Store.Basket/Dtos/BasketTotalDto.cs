using System;
using System.Collections.Generic;
using System.Linq;

namespace Store.Basket.Dtos
{
    public class BasketTotalDto
    {
        public Guid UserId { get; set; }
        public BasketType BasketType { get; set; } = BasketType.Sale;
        public string? DiscountCode { get; set; }
        public int? DiscountRate { get; set; }
        public List<BasketItemDto> BasketItems { get; set; } = new();
        private decimal _totalPrice;
        public decimal TotalPrice 
        { 
            get => BasketItems != null && BasketItems.Any() ? BasketItems.Sum(x => x.Quantity * x.Price) : _totalPrice; 
            set => _totalPrice = value; 
        }
    }
}
