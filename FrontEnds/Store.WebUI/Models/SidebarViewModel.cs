using Store.DtoLayer.CatalogDtos.CategoryDtos;
using Store.DtoLayer.CatalogDtos.ColorDtos;
using Store.DtoLayer.CatalogDtos.SizeDtos;

namespace Store.WebUI.Models
{
    public class SidebarViewModel
    {
        public List<ResultCategoryDto> Categories { get; set; } = new List<ResultCategoryDto>();
        public List<ResultColorDto> Colors { get; set; } = new List<ResultColorDto>();
        public List<ResultSizeDto> Sizes { get; set; } = new List<ResultSizeDto>();
    }
}
