using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _BrandsDefaultComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

