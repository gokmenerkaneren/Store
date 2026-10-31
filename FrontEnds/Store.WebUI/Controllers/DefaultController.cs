using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Services;
using Store.WebUI.Services.Interfaces;
using Store.DtoLayer.LocationDtos.BultenAboneligiDtos;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.WebUI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly ILocationApiService _locationApiService;
        private readonly IServiceHealthCheckService _healthCheckService;

        public DefaultController(
            ILocationApiService locationApiService,
            IServiceHealthCheckService healthCheckService)
        {
            _locationApiService = locationApiService;
            _healthCheckService = healthCheckService;
        }

        public async Task<IActionResult> Index()
        {
            // Kullanıcı isteği: Default sayfası ilk kez başlatıldığında servislerin durumunu kontrol et, logla ve ekranda popup göster
            var report = await _healthCheckService.CheckAndLogAsync("Default Sayfası İlk Açılış");
            if (report.HasOfflineServices)
            {
                ViewBag.OfflineServicesJson = JsonSerializer.Serialize(report.OfflineServices);
                ViewBag.TotalServicesCount = report.Services.Count;
                ViewBag.OfflineServicesCount = report.OfflineServices.Count;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(CreateBultenAboneligiDto createBultenAboneligiDto)
        {
            var response = await _locationApiService.CreateBultenAboneligiAsync(createBultenAboneligiDto);
            if (response.IsSuccessStatusCode)
            {
                Response.Cookies.Append("NewsletterSubscribed", "true", new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = System.DateTimeOffset.UtcNow.AddYears(1)
                });
                return Ok(new { success = true, message = "Bülten aboneliğiniz başarıyla oluşturuldu." });
            }
            else
            {
                return BadRequest(new { success = false, message = "Abonelik işlemi sırasında bir hata oluştu. Lütfen geçerli bir e-posta adresi giriniz." });
            }
        }
    }
}
