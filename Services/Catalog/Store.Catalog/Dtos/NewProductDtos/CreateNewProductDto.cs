using System;

namespace Store.Catalog.Dtos.NewProductDtos
{
    public class CreateNewProductDto
    {
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
