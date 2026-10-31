using System;
namespace Store.DtoLayer.CatalogDtos.DefaultTrendDtos
{
    public class ResultDefaultTrendDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        
        // Joined Product Details
        public string ProductName { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}