namespace Store.Catalog.Dtos.CategoryDtos
{
    public class CreateCategoryDto
    {
        public Guid? ParentId { get; set; }

        public string? Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
        public string? ImageUrl { get; set; }
    }
}

