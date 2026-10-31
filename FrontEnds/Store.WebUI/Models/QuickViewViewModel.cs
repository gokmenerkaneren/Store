using System;
using System.Collections.Generic;

namespace Store.WebUI.Models
{
    public class QuickViewViewModel
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }

        public decimal OldPrice { get; set; }
        public decimal RetailPrice { get; set; }

        public string Slug { get; set; } = string.Empty;

        // Örn: Malzeme, Kolon Malzeme, Boya Cinsi vs.
        public List<KeyValuePair<string, string>> Attributes { get; set; } = new();
    }
}
