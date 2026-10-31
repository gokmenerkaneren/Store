using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.CityDtos;
using Store.DtoLayer.LocationDtos.CountryDtos;
using Store.DtoLayer.LocationDtos.DistrictDtos;
using Store.DtoLayer.LocationDtos.NeighborhoodDtos;
using System.Text;
using Store.WebUI.Models;
using Microsoft.Extensions.Options;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NeighborhoodController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public NeighborhoodController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
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
                ViewBag.Countries = countries?.Where(c => c.IsActive).ToList()
                                    ?? new List<ResultCountryDto>();
            }
            else
            {
                ViewBag.Countries = new List<ResultCountryDto>();
            }
        }

        /// <summary>
        /// City API'sinden aktif şehirleri çekip ViewBag.Cities'e atar.
        /// </summary>
        private async Task LoadCitiesAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/City");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var cities = JsonConvert.DeserializeObject<List<ResultCityDto>>(json);
                ViewBag.Cities = cities?.Where(c => c.IsActive).ToList()
                                    ?? new List<ResultCityDto>();
            }
            else
            {
                ViewBag.Cities = new List<ResultCityDto>();
            }
        }

        /// <summary>
        /// District API'sinden aktif ilçeleri çekip ViewBag.Districts'e atar.
        /// </summary>
        private async Task LoadDistrictsAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/District");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var districts = JsonConvert.DeserializeObject<List<ResultDistrictDto>>(json);
                ViewBag.Districts = districts?.Where(d => d.IsActive).ToList()
                                    ?? new List<ResultDistrictDto>();
            }
            else
            {
                ViewBag.Districts = new List<ResultDistrictDto>();
            }
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            await LoadCountriesAsync();
            await LoadCitiesAsync();
            await LoadDistrictsAsync();

            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;
            ViewBag.ShowDeleted = actualShowDeleted;

            // Filtre ilk girişte boş tablo gösterilecek
            return View(new List<ResultNeighborhoodDto>());
        }

        [HttpGet]
        public async Task<IActionResult> GetNeighborhoods(Guid districtId, bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = $"api/Neighborhood/ByDistrictId/{districtId}" + (actualShowDeleted ? "?showDeleted=true" : "");
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultNeighborhoodDto>>(json);
                return Json(values ?? new List<ResultNeighborhoodDto>());
            }

            return Json(new List<ResultNeighborhoodDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCountriesAsync();
            await LoadCitiesAsync();
            await LoadDistrictsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNeighborhoodDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Neighborhood", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            await LoadCountriesAsync();
            await LoadCitiesAsync();
            await LoadDistrictsAsync();
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Neighborhood/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<ResultNeighborhoodDto>(json);
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateNeighborhoodDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/Neighborhood", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        public async Task<IActionResult> DeleteNeighborhood(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Neighborhood/soft-delete/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RestoreNeighborhood(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"api/Neighborhood/restore/{id}", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Neighborhood", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "Neighborhood", new { area = "Admin", showDeleted = true });
        }
    }
}
