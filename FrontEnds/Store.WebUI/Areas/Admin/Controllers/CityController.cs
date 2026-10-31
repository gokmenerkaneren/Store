using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.CityDtos;
using Store.DtoLayer.LocationDtos.CountryDtos;
using System.Collections.Generic;
using System.Text;
using Store.WebUI.Models;
using Microsoft.Extensions.Options;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CityController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        public CityController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        /// <summary>
        /// Country API'sinden aktif ülkeleri çekip ViewBag.Countries'e atar.
        /// </summary>
        private async Task LoadCountriesAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Country");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<ResultCountryDto>>(json);
                // Sadece aktif olanları gönder
                ViewBag.Countries = countries?.Where(c => c.IsActive).ToList()
                                    ?? new List<ResultCountryDto>();
            }
            else
            {
                ViewBag.Countries = new List<ResultCountryDto>();
            }
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            await LoadCountriesAsync();
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = "api/City" + (actualShowDeleted ? "?showDeleted=true" : "");
            var response = await client.GetAsync(url);
   
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCityDto>>(json);

                if (values != null)
                {
                    var countries = ViewBag.Countries as List<ResultCountryDto> ?? new List<ResultCountryDto>();
                    values = values
                        .OrderBy(c => c.IsDeleted ? 2 : (c.IsActive ? 0 : 1))
                        .ThenBy(c =>
                        {
                            var country = countries.FirstOrDefault(co => co.Id == c.CountryId);
                            return country?.CountryName ?? "";
                        })
                        .ThenBy(c => c.CityName)
                        .ToList();
                }

                return View(values);
            }
            return View(new List<ResultCityDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCountriesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCityDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/City", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            await LoadCountriesAsync();
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/City/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<ResultCityDto>(json);
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCityDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/City", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        public async Task<IActionResult> DeleteCity(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/City/soft-delete/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RestoreCity(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"api/City/restore/{id}", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "City", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "City", new { area = "Admin", showDeleted = true });
        }
    }
}
