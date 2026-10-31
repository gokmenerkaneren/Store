namespace Store.Catalog.Dtos.InstagramPostDtos
{
    public class UpdateInstagramPostDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string PostUrl { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
    }
}

