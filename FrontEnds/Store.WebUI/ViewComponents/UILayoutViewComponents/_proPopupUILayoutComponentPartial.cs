using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _proPopupUILayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
