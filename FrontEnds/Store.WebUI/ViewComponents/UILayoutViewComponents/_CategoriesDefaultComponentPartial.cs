using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.CategoryDtos;
using Store.WebUI.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class CategoryPromoViewModel
    {
        public ResultCategoryDto Parent { get; set; }
        public List<ResultCategoryDto> Children { get; set; }
    }

    public class _CategoriesDefaultComponentPartial : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public _CategoriesDefaultComponentPartial(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allCategories = await _categoryService.GetAllCategoriesAsync();
            var activeCategories = allCategories.Where(x => x.IsActive).ToList();

            var parents = activeCategories.Where(x => x.ParentId == null).ToList();
            
            var model = new List<CategoryPromoViewModel>();
            foreach (var parent in parents)
            {
                model.Add(new CategoryPromoViewModel
                {
                    Parent = parent,
                    Children = activeCategories.Where(x => x.ParentId == parent.Id).ToList()
                });
            }

            return View(model);
        }
    }
}

