using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Areas.Admin.ViewComponents.AdminLayoutViewComponets
{
    public class _AdminLayoutComponentCardPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
