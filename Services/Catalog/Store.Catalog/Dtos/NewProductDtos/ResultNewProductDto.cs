using System;

namespace Store.Catalog.Dtos.NewProductDtos
{
    public class ResultNewProductDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
    }
}
