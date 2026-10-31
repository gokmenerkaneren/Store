using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.CatalogDtos.PriceTypeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PriceTypeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public PriceTypeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");

            var url = "http://localhost:7301/api/PriceTypes" + (actualShowDeleted ? "?showDeleted=true" : "");
            var responseMessage = await client.GetAsync(url);

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultPriceTypeDto>>(jsonData);

                if (values != null)
                {
                    values = values
                        .OrderBy(m => m.IsDeleted ? 2 : (m.IsActive ? 0 : 1))
                        .ThenBy(m => m.Name)
                        .ToList();
                }

                return View(values);
            }

            return View(new List<ResultPriceTypeDto>());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePriceType(CreatePriceTypeDto createPriceTypeDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonData = JsonConvert.SerializeObject(createPriceTypeDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("http://localhost:7301/api/PriceTypes", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            return Json(new { success = false, message = errorMessage });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePriceType(UpdatePriceTypeDto updatePriceTypeDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonData = JsonConvert.SerializeObject(updatePriceTypeDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("http://localhost:7301/api/PriceTypes", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            return Json(new { success = false, message = errorMessage });
        }

        public async Task<IActionResult> DeletePriceType(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.DeleteAsync($"http://localhost:7301/api/PriceTypes/{id}");
            return RedirectToAction("Index", "PriceType", new { area = "Admin" });
        }
        
        public async Task<IActionResult> RestorePriceType(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PostAsync($"http://localhost:7301/api/PriceTypes/Restore/{id}", null);
            return RedirectToAction("Index", "PriceType", new { area = "Admin" });
        }
    }
}
