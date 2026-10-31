using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Areas.Admin.Controllers
{
    public class DefaultPromoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
