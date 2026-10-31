using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Controllers
{
    public class UILayoutController : Controller
    {
        public IActionResult _UILayout()
        {
            return View();
        }
    }
}
