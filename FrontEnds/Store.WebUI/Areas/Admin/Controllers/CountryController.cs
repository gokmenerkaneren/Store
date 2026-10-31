using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.CountryDtos;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Store.WebUI.Models;
namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CountryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        public CountryController(IHttpClientFactory httpClientFactory, Microsoft.Extensions.Options.IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            // Sadece yetkili (Admin) kullanıcılar silinen kayıtları görebilir
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");

            var url = "api/Country" + (actualShowDeleted ? "?showDeleted=true" : "");
            var responseMessage = await client.GetAsync(url);

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCountryDto>>(jsonData);

                if (values != null)
                {
                    values = values
                        .OrderBy(c => c.IsDeleted ? 2 : (c.IsActive ? 0 : 1))
                        .ThenBy(c => c.CountryName)
                        .ThenBy(c => c.CountryCode)
                        .ToList();
                }

                return View(values);
            }

            return View(new List<ResultCountryDto>());
        }
            
            [HttpGet]
            public IActionResult CreateCountry()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> CreateCountry(CreateCountryDto createCountryDto)
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var jsonData = JsonConvert.SerializeObject(createCountryDto);
                StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync($"{_apiSettings.LocationBaseUrl}/api/Country", stringContent);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }

                var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                return Json(new { success = false, message = errorMessage });
            }

            [HttpPost]
            public async Task<IActionResult> UpdateCountry(UpdateCountryDto updateCountryDto)
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var jsonData = JsonConvert.SerializeObject(updateCountryDto);
                StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PutAsync($"{_apiSettings.LocationBaseUrl}/api/Country", stringContent);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }

                var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                return Json(new { success = false, message = errorMessage });
            }

        

        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            // Soft‑delete via dedicated endpoint
            var responseMessage = await client.DeleteAsync($"{_apiSettings.LocationBaseUrl}/api/Country/soft-delete/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Country", new { area = "Admin" });
            }
            // If soft‑delete fails, still attempt to show index with error handling in view
            return RedirectToAction("Index", "Country", new { area = "Admin" });
        }

        public async Task<IActionResult> SoftDeleteCountry(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.DeleteAsync($"{_apiSettings.LocationBaseUrl}/api/Country/soft-delete/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Country", new { area = "Admin" });
            }
            return RedirectToAction("Index", "Country", new { area = "Admin" });
        }

        public async Task<IActionResult> RestoreCountry(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"{_apiSettings.LocationBaseUrl}/api/Country/restore/{id}", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Country", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "Country", new { area = "Admin", showDeleted = true });
        }
    }
}
