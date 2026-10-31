using System;
using System.Collections.Generic;

namespace Store.WebUI.Models.CheckoutViewModels
{
    public class OrderSuccessViewModel
    {
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerFullName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<CheckoutItemViewModel> Items { get; set; } = new();
    }
}

