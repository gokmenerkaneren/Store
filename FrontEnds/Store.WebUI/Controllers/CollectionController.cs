using Microsoft.AspNetCore.Mvc;

namespace Store.WebUI.Controllers
{
    public class CollectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
