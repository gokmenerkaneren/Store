using System;
namespace Store.Catalog.Dtos.DefaultTrendDtos
{
    public class GetByIdDefaultTrendDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
    }
}