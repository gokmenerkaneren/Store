using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Models.Rental;
using Store.WebUI.Services.Interfaces;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Store.WebUI.Controllers
{
    [Authorize]
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        public IActionResult Index(string productId, decimal price)
        {
            var model = new CreateRentalViewModel();
            if (!string.IsNullOrEmpty(productId))
            {
                model.Items.Add(new RentalItemViewModel 
                { 
                    ProductId = productId, 
                    Quantity = 1, 
                    UnitPrice = price 
                });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CreateRentalViewModel model)
        {
            try
            {
                await _rentalService.CreateRentalOrderAsync(model);
                TempData["SuccessMessage"] = "Kiralama talebiniz başarıyla alındı!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Seçili tarihlerde ürün müsait değil: " + ex.Message;
                return View(model);
            }
        }
    }
}
