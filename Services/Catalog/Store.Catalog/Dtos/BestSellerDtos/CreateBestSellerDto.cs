using System;

namespace Store.Catalog.Dtos.BestSellerDtos
{
    public class CreateBestSellerDto
    {
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
    }
}
