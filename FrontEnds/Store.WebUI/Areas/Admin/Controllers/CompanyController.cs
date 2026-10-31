using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Store.DtoLayer.LocationDtos.CompanyDtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Store.WebUI.Models;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("[area]/[controller]/[action]")]
    public class CompanyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApiSettings _apiSettings;

        public CompanyController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = "api/Company?showDeleted=" + actualShowDeleted.ToString().ToLower();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCompanyDto>>(json);

                if (values != null)
                {
                    values = values
                        .OrderBy(c => c.IsDeleted ? 2 : (c.IsActive ? 0 : 1))
                        .ThenBy(c => c.LegalName ?? "")
                        .ThenBy(c => c.BrandName ?? "")
                        .ToList();
                }

                return View(values);
            }
            return View(new List<ResultCompanyDto>());
        }

        [HttpGet]
        public IActionResult Add()
        {
            var newCompany = new UpdateCompanyDto
            {
                IsActive = true
            };
            return View("AddOrEdit", newCompany);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Company/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateCompanyDto>(json);
                return View("AddOrEdit", value);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Add(UpdateCompanyDto dto, IFormFile? logoFile)
        {
            return await SaveCompany(dto, logoFile);
        }

        [HttpPost("{id?}")]
        public async Task<IActionResult> Edit(UpdateCompanyDto dto, IFormFile? logoFile)
        {
            return await SaveCompany(dto, logoFile);
        }

        private async Task<IActionResult> SaveCompany(UpdateCompanyDto dto, IFormFile? logoFile)
        {
            if (logoFile != null && logoFile.Length > 0)
            {
                var extension = Path.GetExtension(logoFile.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;
                var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Companies");
                
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                
                var path = Path.Combine(directoryPath, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await logoFile.CopyToAsync(stream);
                }
                dto.LogoUrl = "/Images/Companies/" + fileName;
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (dto.Id == Guid.Empty)
            {
                response = await client.PostAsync("api/Company", content);
            }
            else
            {
                response = await client.PutAsync("api/Company", content);
            }

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            ModelState.AddModelError("", err);
            return View("AddOrEdit", dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Company/soft-delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> RestoreCompany(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"api/Company/restore/{id}", new StringContent(string.Empty));
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Company", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "Company", new { area = "Admin", showDeleted = true });
        }
    }
}
