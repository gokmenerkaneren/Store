using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _proBreadcrumbUILayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke(string productName = null)
        {
            ViewBag.ProductName = productName;
            return View();
        }
    }
}
