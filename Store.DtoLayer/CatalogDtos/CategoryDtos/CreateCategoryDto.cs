using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DtoLayer.CatalogDtos.CategoryDtos
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
