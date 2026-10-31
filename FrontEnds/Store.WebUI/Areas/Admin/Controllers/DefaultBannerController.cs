using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Areas.Admin.Controllers
{
    public class DefaultBannerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
