using System;

namespace Store.Catalog.Dtos.BestSellerDtos
{
    public class GetByIdBestSellerDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
    }
}
