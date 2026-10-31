using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _colBreadcrumbUILayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

