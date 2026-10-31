using System;

namespace Store.DtoLayer.CatalogDtos.BestSellerDtos
{
    public class ResultBestSellerDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int OrderIndex { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
    }
}
