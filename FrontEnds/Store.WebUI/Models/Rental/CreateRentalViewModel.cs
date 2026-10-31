using System;
using System.Collections.Generic;

namespace Store.WebUI.Models.Rental
{
    public class CreateRentalViewModel
    {
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        public List<RentalItemViewModel> Items { get; set; } = new List<RentalItemViewModel>();
    }

    public class RentalItemViewModel
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
