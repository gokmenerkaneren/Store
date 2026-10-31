using System;

namespace Store.Catalog.Dtos.BestSellerDtos
{
    public class UpdateBestSellerDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
    }
}
