using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _proRelatedUILayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
