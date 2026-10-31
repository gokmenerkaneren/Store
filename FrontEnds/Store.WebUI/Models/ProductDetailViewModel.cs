using System;
using System.Collections.Generic;

namespace Store.WebUI.Models
{
    public class ProductDetailViewModel
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }

        public decimal OldPrice { get; set; }
        public decimal RetailPrice { get; set; }

        public string Slug { get; set; } = string.Empty;

        // 1 No'lu Alan: Marka
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }

        // 3 No'lu Alan: Notlar
        public string? Notes { get; set; }

        // Örn: Malzeme, Kolon Malzeme, Boya Cinsi vs.
        public List<KeyValuePair<string, string>> Attributes { get; set; } = new();
    }
}
