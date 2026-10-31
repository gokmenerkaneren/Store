using Store.Catalog.Entities;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Dtos.CategoryDtos
{
    public class GetByIdCategoryDto : BaseEntity
    {

        public Guid? ParentId { get; set; }

        public string? Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}

