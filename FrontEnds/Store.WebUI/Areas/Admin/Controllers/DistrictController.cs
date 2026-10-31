using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.CityDtos;
using Store.DtoLayer.LocationDtos.CountryDtos;
using Store.DtoLayer.LocationDtos.DistrictDtos;
using System.Text;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DistrictController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DistrictController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
                // Sadece aktif olanları gönder
                ViewBag.Cities = cities?.Where(c => c.IsActive).ToList()
                                    ?? new List<ResultCityDto>();
            }
            else
            {
                ViewBag.Cities = new List<ResultCityDto>();
            }
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            await LoadCountriesAsync();
            await LoadCitiesAsync();
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = "api/District" + (actualShowDeleted ? "?showDeleted=true" : "");
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultDistrictDto>>(json);

                if (values != null)
                {
                    var cityList = ViewBag.Cities as List<ResultCityDto> ?? new List<ResultCityDto>();
                    var countryList = ViewBag.Countries as List<ResultCountryDto> ?? new List<ResultCountryDto>();

                    values = values
                        .OrderBy(d => d.IsDeleted ? 2 : (d.IsActive ? 0 : 1))
                        .ThenBy(d =>
                        {
                            var city = cityList.FirstOrDefault(c => c.Id == d.CityId);
                            var country = city != null ? countryList.FirstOrDefault(co => co.Id == city.CountryId) : null;
                            return country?.CountryName ?? "";
                        })
                        .ThenBy(d =>
                        {
                            var city = cityList.FirstOrDefault(c => c.Id == d.CityId);
                            return city?.CityName ?? "";
                        })
                        .ThenBy(d => d.DistrictName)
                        .ToList();
                }

                return View(values);
            }
            return View(new List<ResultDistrictDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCountriesAsync();
            await LoadCitiesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDistrictDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/District", content);
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
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/District/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<ResultDistrictDto>(json);
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateDistrictDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/District", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        public async Task<IActionResult> DeleteDistrict(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/District/soft-delete/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));
            
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RestoreDistrict(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"api/District/restore/{id}", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "District", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "District", new { area = "Admin", showDeleted = true });
        }
    }
}
