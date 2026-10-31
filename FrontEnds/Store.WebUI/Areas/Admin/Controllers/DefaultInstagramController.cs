using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Areas.Admin.Controllers
{
    public class DefaultInstagramController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
