using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.DealerDtos;
using Store.DtoLayer.LocationDtos.DealerAddressDtos;
using Store.DtoLayer.LocationDtos.ContactInfoDtos;
using Store.DtoLayer.LocationDtos.CountryDtos;
using Store.DtoLayer.LocationDtos.CityDtos;
using Store.DtoLayer.LocationDtos.DistrictDtos;
using Store.DtoLayer.LocationDtos.NeighborhoodDtos;
using System.Collections.Generic;
using System.Text;
using Store.WebUI.Models;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DealerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public DealerController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        private async Task LoadDropdownsAndAddressesAsync(Guid? dealerId = null)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // Initialize defaults to prevent NullReferenceExceptions in views
            ViewBag.ContactInfos = new List<ResultContactInfoDto>();
            ViewBag.Countries = new List<ResultCountryDto>();
            ViewBag.DealerAddresses = new List<ResultDealerAddressDto>();

            try
            {
                // 1. İletişim Bilgileri (ContactInfos)
                var contactResponse = await client.GetAsync("api/ContactInfo");
                if (contactResponse.IsSuccessStatusCode)
                {
                    var json = await contactResponse.Content.ReadAsStringAsync();
                    var contactInfos = JsonConvert.DeserializeObject<List<ResultContactInfoDto>>(json);
                    ViewBag.ContactInfos = contactInfos?.Where(c => c.IsActive && !c.IsDeleted)
                                             .OrderBy(c => c.AuthorizedPersonName)
                                             .ToList() ?? new List<ResultContactInfoDto>();
                }

                // 2. Ülkeler (Countries)
                var countryResponse = await client.GetAsync("api/Country");
                if (countryResponse.IsSuccessStatusCode)
                {
                    var json = await countryResponse.Content.ReadAsStringAsync();
                    var allCountries = JsonConvert.DeserializeObject<List<ResultCountryDto>>(json);
                    ViewBag.Countries = allCountries?.Where(c => c.IsActive)
                                          .OrderBy(c => c.CountryName)
                                          .ToList() ?? new List<ResultCountryDto>();
                }

                // 3. Bayi Adresleri (Eğer Güncelleme modundaysa)
                if (dealerId.HasValue && dealerId.Value != Guid.Empty)
                {
                    var addressResponse = await client.GetAsync($"api/DealerAddress/by-dealer/{dealerId.Value}");
                    if (addressResponse.IsSuccessStatusCode)
                    {
                        var json = await addressResponse.Content.ReadAsStringAsync();
                        ViewBag.DealerAddresses = JsonConvert.DeserializeObject<List<ResultDealerAddressDto>>(json) ?? new List<ResultDealerAddressDto>();
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Add proper logging mechanism here
                Console.WriteLine($"Error loading dropdowns and addresses: {ex.Message}");
            }
        }

        public IActionResult Index(bool showDeleted = false)
        {
            ViewBag.ShowDeleted = showDeleted;
            return View(); // Don't fetch data, view will fetch it via AJAX
        }

        [HttpPost]
        public async Task<IActionResult> GetDataTablesData([FromForm] Store.DtoLayer.CommonDtos.DataTablesRequest request, bool showDeleted = false)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"api/Dealer/datatables?showDeleted={showDeleted.ToString().ToLower()}";
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Store.DtoLayer.CommonDtos.DataTablesResponse<ResultDealerDto>>(responseJson);
                return Json(data);
            }
            return Json(new { draw = request.Draw, recordsTotal = 0, recordsFiltered = 0, data = new List<object>() });
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await LoadDropdownsAndAddressesAsync();
            var newDealer = new UpdateDealerDto
            {
                IsActive = true
            };
            return View("AddOrEdit", newDealer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            await LoadDropdownsAndAddressesAsync(id);
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Dealer/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateDealerDto>(json);
                return View("AddOrEdit", value);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Add(UpdateDealerDto dto)
        {
            return await SaveDealer(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateDealerDto dto)
        {
            return await SaveDealer(dto);
        }

        private async Task<IActionResult> SaveDealer(UpdateDealerDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var allDealersResponse = await client.GetAsync("api/Dealer");
            if (allDealersResponse.IsSuccessStatusCode)
            {
                var allJson = await allDealersResponse.Content.ReadAsStringAsync();
                var allDealers = JsonConvert.DeserializeObject<List<ResultDealerDto>>(allJson) ?? new List<ResultDealerDto>();

                var duplicateCompany = allDealers.FirstOrDefault(x => x.Id != dto.Id && !string.IsNullOrEmpty(x.CompanyName) && x.CompanyName.ToLower() == dto.CompanyName.ToLower());
                if (duplicateCompany != null)
                {
                    ModelState.AddModelError("CompanyName", "Bu ticari unvan zaten kullanımda.");
                }

                var duplicateTax = allDealers.FirstOrDefault(x => x.Id != dto.Id && !string.IsNullOrEmpty(x.TaxNumber) && x.TaxNumber == dto.TaxNumber);
                if (duplicateTax != null)
                {
                    ModelState.AddModelError("TaxNumber", "Bu vergi numarası zaten kullanımda.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAndAddressesAsync(dto.Id);
                return View("AddOrEdit", dto);
            }


            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (dto.Id == Guid.Empty)
            {
                response = await client.PostAsync("api/Dealer", content);
            }
            else
            {
                response = await client.PutAsync("api/Dealer", content);
            }

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            ModelState.AddModelError("", err);
            await LoadDropdownsAndAddressesAsync(dto.Id);
            return View("AddOrEdit", dto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveDealerAddress([FromBody] UpdateDealerAddressDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (dto.Id == Guid.Empty)
            {
                // Create
                response = await client.PostAsync("api/DealerAddress", content);
            }
            else
            {
                // Update
                response = await client.PutAsync("api/DealerAddress", content);
            }

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Adres başarıyla kaydedildi." });
            }

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        [HttpGet]
        public async Task<IActionResult> GetDealerAddressById(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/DealerAddress/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> SoftDeleteDealerAddress(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/DealerAddress/soft-delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Adres başarıyla silindi." });
            }
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        // ── Cascading Dropdown AJAX Endpoints ──────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetCitiesByCountry(Guid countryId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/City");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var allCities = JsonConvert.DeserializeObject<List<ResultCityDto>>(json) ?? new List<ResultCityDto>();
                var filtered = allCities
                    .Where(c => c.CountryId == countryId && c.IsActive && !c.IsDeleted)
                    .OrderBy(c => c.CityName)
                    .Select(c => new { id = c.Id, name = c.CityName })
                    .ToList();
                return Json(filtered);
            }
            return Json(new List<object>());
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(Guid cityId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/District");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var allDistricts = JsonConvert.DeserializeObject<List<ResultDistrictDto>>(json) ?? new List<ResultDistrictDto>();
                var filtered = allDistricts
                    .Where(d => d.CityId == cityId && d.IsActive && !d.IsDeleted)
                    .OrderBy(d => d.DistrictName)
                    .Select(d => new { id = d.Id, name = d.DistrictName })
                    .ToList();
                return Json(filtered);
            }
            return Json(new List<object>());
        }

        [HttpGet]
        public async Task<IActionResult> GetNeighborhoodsByDistrict(Guid districtId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Neighborhood");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var allNeighborhoods = JsonConvert.DeserializeObject<List<ResultNeighborhoodDto>>(json) ?? new List<ResultNeighborhoodDto>();
                var filtered = allNeighborhoods
                    .Where(n => n.DistrictId == districtId && n.IsActive && !n.IsDeleted)
                    .OrderBy(n => n.NeighborhoodName)
                    .Select(n => new { id = n.Id, name = n.NeighborhoodName })
                    .ToList();
                return Json(filtered);
            }
            return Json(new List<object>());
        }

        // ── CRUD ──────────────────────────────────────────────────────

        public async Task<IActionResult> DeleteDealer(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Dealer/soft-delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RestoreDealer(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"api/Dealer/restore/{id}", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Dealer", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "Dealer", new { area = "Admin", showDeleted = true });
        }
    }
}
