namespace Store.DtoLayer.CatalogDtos.InstagramPostDtos
{
    public class ResultInstagramPostDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string PostUrl { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
    }
}

