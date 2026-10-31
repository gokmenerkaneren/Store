namespace Store.DtoLayer.CatalogDtos.CategoryDtos
{
    public class UpdateCategoryDto
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string? Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
        public string? ImageUrl { get; set; }
    }
}
