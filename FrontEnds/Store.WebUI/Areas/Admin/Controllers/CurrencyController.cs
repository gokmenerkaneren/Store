using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.CatalogDtos.CurrencyDtos;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CurrencyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CurrencyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");

            var url = "http://localhost:7301/api/Currencies" + (actualShowDeleted ? "?showDeleted=true" : "");
            var responseMessage = await client.GetAsync(url);

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCurrencyDto>>(jsonData);

                if (values != null)
                {
                    values = values
                        .OrderBy(c => c.IsDeleted ? 2 : (c.IsActive ? 0 : 1))
                        .ThenBy(c => c.Name)
                        .ToList();
                }

                return View(values);
            }

            return View(new List<ResultCurrencyDto>());
        }
            
        [HttpGet]
        public IActionResult CreateCurrency()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCurrency(CreateCurrencyDto createCurrencyDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonData = JsonConvert.SerializeObject(createCurrencyDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("http://localhost:7301/api/Currencies", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            return Json(new { success = false, message = errorMessage });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCurrency(UpdateCurrencyDto updateCurrencyDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonData = JsonConvert.SerializeObject(updateCurrencyDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("http://localhost:7301/api/Currencies", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            return Json(new { success = false, message = errorMessage });
        }

        public async Task<IActionResult> DeleteCurrency(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.DeleteAsync($"http://localhost:7301/api/Currencies/{id}");
            return RedirectToAction("Index", "Currency", new { area = "Admin" });
        }
        
        public async Task<IActionResult> RestoreCurrency(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PostAsync($"http://localhost:7301/api/Currencies/Restore/{id}", null);
            return RedirectToAction("Index", "Currency", new { area = "Admin" });
        }
    }
}
