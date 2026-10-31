using System;

namespace Store.DtoLayer.CatalogDtos.BestSellerDtos
{
    public class CreateBestSellerDto
    {
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
    }
}
