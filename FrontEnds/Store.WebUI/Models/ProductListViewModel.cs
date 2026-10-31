using System;
using System.Collections.Generic;

namespace Store.WebUI.Models
{
    public class ProductListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal RetailPrice { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public string HoverImageUrl { get; set; } = string.Empty;
        public List<string> Colors { get; set; } = new List<string> { "Beyaz", "Kırmızı" };
        public List<string> Sizes { get; set; } = new List<string> { "40", "42", "44" };
        public decimal OldPrice { get; set; } = 1234m;
    }
}
