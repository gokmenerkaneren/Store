using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _SliderDefaultComponentPartial : ViewComponent
    {
        private readonly IDefaultSlider1Service _sliderService;

        public _SliderDefaultComponentPartial(IDefaultSlider1Service sliderService)
        {
            _sliderService = sliderService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sliders = await _sliderService.GetAllAsync();
            var activeSliders = sliders.Where(x => x.IsActive).OrderBy(x => x.Sirano).ToList();
            return View(activeSliders);
        }
    }
}

