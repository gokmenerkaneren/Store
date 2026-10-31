using System;
namespace Store.DtoLayer.CatalogDtos.DefaultTrendDtos
{
    public class UpdateDefaultTrendDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
    }
}