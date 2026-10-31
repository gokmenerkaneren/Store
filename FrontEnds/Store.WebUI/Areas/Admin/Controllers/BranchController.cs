using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.BranchDtos;
using Store.DtoLayer.LocationDtos.CompanyDtos;
using System.Collections.Generic;
using System.Text;
using Store.WebUI.Models;
using Microsoft.Extensions.Options;


namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BranchController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        public BranchController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        /// <summary>
        /// Aktif şirketleri Company API'sinden çekip ViewBag.Companies'e atar.
        /// </summary>
        private async Task LoadCompaniesAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Company");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var companies = JsonConvert.DeserializeObject<List<ResultCompanyDto>>(json);
                ViewBag.Companies = companies?.Where(c => c.IsActive && !c.IsDeleted)
                                        .OrderBy(c => c.LegalName)
                                        .ToList()
                                    ?? new List<ResultCompanyDto>();
            }
            else
            {
                ViewBag.Companies = new List<ResultCompanyDto>();
            }
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = $"api/Branch?showDeleted={actualShowDeleted.ToString().ToLower()}";
            var response = await client.GetAsync(url);

            // Şirket listesini yükle (tabloda CompanyId yerine isim göstermek için)
            await LoadCompaniesAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultBranchDto>>(json);

                if (values != null)
                {
                    var companyList = ViewBag.Companies as List<ResultCompanyDto> ?? new List<ResultCompanyDto>();
                    values = values
                        .OrderBy(b => b.IsDeleted ? 2 : (b.IsActive ? 0 : 1))
                        .ThenBy(b =>
                        {
                            var company = companyList.FirstOrDefault(c => c.Id == b.CompanyId);
                            return company?.LegalName ?? "";
                        })
                        .ThenBy(b => b.BranchType ?? "")
                        .ThenBy(b => b.Name ?? "")
                        .ToList();
                }

                return View(values);
            }
            return View(new List<ResultBranchDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //await LoadCompaniesAsync();
            //return View("AddOrEdit", new UpdateBranchDto());

            await LoadCompaniesAsync();
            ViewBag.Countries = "CompanyAdd";
            var newBranch = new UpdateBranchDto
            {
                IsActive = true      // <‑‑ burada varsayılan “Aktif” seçilir
            };
            return View("AddOrEdit", newBranch);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            await LoadCompaniesAsync();
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Branch/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateBranchDto>(json);
                return View("AddOrEdit", value);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Add(UpdateBranchDto dto)
        {
            return await SaveBranch(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBranchDto dto)
        {
            return await SaveBranch(dto);
        }

        private async Task<IActionResult> SaveBranch(UpdateBranchDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (dto.Id == Guid.Empty)
            {
                response = await client.PostAsync("api/Branch", content);
            }
            else
            {
                response = await client.PutAsync("api/Branch", content);
            }

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            ModelState.AddModelError("", err);
            await LoadCompaniesAsync();
            return View("AddOrEdit", dto);
        }

        public async Task<IActionResult> DeleteBranch(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Branch/soft-delete/{id}");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RestoreBranch(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"api/Branch/restore/{id}", new StringContent(string.Empty));
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Branch", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "Branch", new { area = "Admin", showDeleted = true });
        }
    }
}
