using MediatR;
//using Store.DtoLayer.CatalogDtos.BrandDtos;
//using Store.DtoLayer.CatalogDtos.CategoryDtos;
using Store.DtoLayer.CatalogDtos.MeasurementDtos;
using System.Collections.Generic;

namespace Store.DtoLayer.CatalogDtos.ProductDtos
{
    public class CreateProductCommand : IRequest<string> // returns created product Id
    {
        // properties needed for product creation (same as old CreateProductDto)
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int MeasurementId { get; set; }
        public List<string> Images { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
