using System;

namespace Store.DtoLayer.CatalogDtos.NewProductDtos
{
    public class CreateNewProductDto
    {
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
