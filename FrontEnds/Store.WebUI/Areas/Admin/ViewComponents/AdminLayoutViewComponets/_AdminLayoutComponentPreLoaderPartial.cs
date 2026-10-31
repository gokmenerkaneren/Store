using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Areas.Admin.ViewComponents.AdminLayoutViewComponets
{
    public class _AdminLayoutComponentPreLoaderPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
