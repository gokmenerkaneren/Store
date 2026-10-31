using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Models;
using Store.WebUI.Services.Interfaces;
using System.Threading.Tasks;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _colSidebarUILayoutComponentPartial : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;

        public _colSidebarUILayoutComponentPartial(
            ICategoryService categoryService,
            IColorService colorService,
            ISizeService sizeService)
        {
            _categoryService = categoryService;
            _colorService = colorService;
            _sizeService = sizeService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var colors = await _colorService.GetAllColorsAsync();
            var sizes = await _sizeService.GetAllSizesAsync();

            var viewModel = new SidebarViewModel
            {
                Categories = categories,
                Colors = colors,
                Sizes = sizes
            };

            return View(viewModel);
        }
    }
}
