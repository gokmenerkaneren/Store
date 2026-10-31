using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _proDetailUILayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke(Store.WebUI.Models.ProductDetailViewModel model)
        {
            return View(model);
        }
    }
}
