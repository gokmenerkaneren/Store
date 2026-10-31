using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.CatalogDtos.MeasurementDtos;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class MeasurementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MeasurementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");

            var url = "http://localhost:7301/api/Measurements" + (actualShowDeleted ? "?showDeleted=true" : "");
            var responseMessage = await client.GetAsync(url);

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultMeasurementDto>>(jsonData);

                if (values != null)
                {
                    values = values
                        .OrderBy(m => m.IsDeleted ? 2 : (m.IsActive ? 0 : 1))
                        .ThenBy(m => m.Name)
                        .ToList();
                }

                return View(values);
            }

            return View(new List<ResultMeasurementDto>());
        }
            
        [HttpGet]
        public IActionResult CreateMeasurement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeasurement(CreateMeasurementDto createMeasurementDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonData = JsonConvert.SerializeObject(createMeasurementDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("http://localhost:7301/api/Measurements", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            return Json(new { success = false, message = errorMessage });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMeasurement(UpdateMeasurementDto updateMeasurementDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonData = JsonConvert.SerializeObject(updateMeasurementDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("http://localhost:7301/api/Measurements", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            return Json(new { success = false, message = errorMessage });
        }

        public async Task<IActionResult> DeleteMeasurement(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.DeleteAsync($"http://localhost:7301/api/Measurements/{id}");
            return RedirectToAction("Index", "Measurement", new { area = "Admin" });
        }
        
        public async Task<IActionResult> RestoreMeasurement(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PostAsync($"http://localhost:7301/api/Measurements/Restore/{id}", null);
            return RedirectToAction("Index", "Measurement", new { area = "Admin" });
        }
    }
}
