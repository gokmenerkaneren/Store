namespace Store.DtoLayer.CatalogDtos.InstagramPostDtos
{
    public class CreateInstagramPostDto
    {
        public string ImageUrl { get; set; }
        public string PostUrl { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
    }
}
