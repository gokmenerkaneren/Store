using System;
namespace Store.DtoLayer.CatalogDtos.DefaultTrendDtos
{
    public class CreateDefaultTrendDto
    {
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; } = true;
    }
}