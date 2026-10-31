
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.ContactInfoDtos;
using System.Collections.Generic;
using System.Text;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ContactInfoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ContactInfoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = "http://localhost:7303/api/ContactInfo?showDeleted=" + showDeleted.ToString().ToLower();
            var response = await client.GetAsync(url);
            ViewBag.ShowDeleted = showDeleted;
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultContactInfoDto>>(json);
                return View(values);
            }
            return View(new List<ResultContactInfoDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContactInfoDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:7303/api/ContactInfo", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateContactInfoDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("http://localhost:7303/api/ContactInfo", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            return Json(new { success = false, message = err });
        }

        public async Task<IActionResult> DeleteContactInfo(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"http://localhost:7303/api/ContactInfo/soft-delete/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
