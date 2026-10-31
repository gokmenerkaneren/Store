using Microsoft.AspNetCore.Mvc;

using Store.WebUI.Services.Interfaces;
using Store.DtoLayer.LocationDtos.MesajlarDtos;
using System.Threading.Tasks;

namespace Store.WebUI.Controllers
{
    public class IletisimController : Controller
    {
        private readonly ILocationApiService _locationApiService;

        public IletisimController(ILocationApiService locationApiService)
        {
            _locationApiService = locationApiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMesajlarDto createMesajlarDto)
        {
            var response = await _locationApiService.CreateMesajlarAsync(createMesajlarDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mesajınız başarıyla alınmıştır. En kısa sürede sizinle iletişime geçeceğiz.";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Mesajınız gönderilirken bir hata oluştu. Lütfen tekrar deneyiniz.";
            return RedirectToAction("Index");
        }
    }
}
