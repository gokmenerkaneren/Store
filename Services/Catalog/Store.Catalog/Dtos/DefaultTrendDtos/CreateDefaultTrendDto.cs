using System;
namespace Store.Catalog.Dtos.DefaultTrendDtos
{
    public class CreateDefaultTrendDto
    {
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; } = true;
    }
}