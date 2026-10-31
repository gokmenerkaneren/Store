using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Areas.Admin.Controllers
{
    public class DefaultCategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
