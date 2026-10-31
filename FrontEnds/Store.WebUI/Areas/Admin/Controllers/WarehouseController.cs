using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.WarehouseDtos;
using Store.DtoLayer.LocationDtos.BranchDtos;
using Store.DtoLayer.LocationDtos.ContactInfoDtos;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public WarehouseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task LoadDropdownDataAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // 1. Şubeler
            var branchResponse = await client.GetAsync("api/Branch");
            if (branchResponse.IsSuccessStatusCode)
            {
                var json = await branchResponse.Content.ReadAsStringAsync();
                var branches = JsonConvert.DeserializeObject<List<ResultBranchDto>>(json);
                ViewBag.Branches = branches?.Where(c => c.IsActive && !c.IsDeleted)
                                        .OrderBy(c => c.Name)
                                        .ToList()
                                    ?? new List<ResultBranchDto>();
            }
            else
            {
                ViewBag.Branches = new List<ResultBranchDto>();
            }

            // 2. İletişim Bilgileri (ContactInfos)
            var contactResponse = await client.GetAsync("api/ContactInfo");
            if (contactResponse.IsSuccessStatusCode)
            {
                var json = await contactResponse.Content.ReadAsStringAsync();
                var contactInfos = JsonConvert.DeserializeObject<List<ResultContactInfoDto>>(json);
                ViewBag.ContactInfos = contactInfos?.Where(c => c.IsActive && !c.IsDeleted)
                                            .OrderBy(c => c.AuthorizedPersonName)
                                            .ToList()
                                        ?? new List<ResultContactInfoDto>();
            }
            else
            {
                ViewBag.ContactInfos = new List<ResultContactInfoDto>();
            }
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            await LoadDropdownDataAsync();

            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            ViewBag.ShowDeleted = actualShowDeleted;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = "api/Warehouse?showDeleted=" + actualShowDeleted.ToString().ToLower();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultWarehouseDto>>(json);

                if (values != null)
                {
                    var branchList = ViewBag.Branches as List<ResultBranchDto> ?? new List<ResultBranchDto>();
                    values = values
                        .OrderBy(w => w.IsDeleted ? 2 : (w.IsActive ? 0 : 1))
                        .ThenBy(w =>
                        {
                            var branch = branchList.FirstOrDefault(b => b.Id == w.BranchId);
                            return branch?.Name ?? "";
                        })
                        .ThenByDescending(w => w.IsDefault)
                        .ThenBy(w => w.Name ?? "")
                        .ToList();
                }

                return View(values);
            }
            return View(new List<ResultWarehouseDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await LoadDropdownDataAsync();
            ViewBag.Warehouses = "CompanyAdd";
            var newWarehouse = new UpdateWarehouseDto
            {
                IsActive = true
            };
            return View("AddOrEdit", newWarehouse);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            await LoadDropdownDataAsync();
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Warehouse/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateWarehouseDto>(json);
                if (value != null)
                {
                    return View("AddOrEdit", value);
                }
                TempData["ErrorMessage"] = "Depo verisi okunamadı (deserializasyon hatası).";
                return RedirectToAction(nameof(Index));
            }
            var errorMsg = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = $"Depo bilgisi alınamadı: {errorMsg}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Add(UpdateWarehouseDto dto)
        {
            return await SaveWarehouse(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateWarehouseDto dto)
        {
            return await SaveWarehouse(dto);
        }

        private async Task<IActionResult> SaveWarehouse(UpdateWarehouseDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View("AddOrEdit", dto);
            }


            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (dto.Id == Guid.Empty)
            {
                response = await client.PostAsync("api/Warehouse", content);
            }
            else
            {
                response = await client.PutAsync("api/Warehouse", content);
            }

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            await LoadDropdownDataAsync();
            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return View("AddOrEdit", dto);
        }

        public async Task<IActionResult> DeleteWarehouse(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Warehouse/soft-delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RestoreWarehouse(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var responseMessage = await client.PutAsync($"api/Warehouse/restore/{id}", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Warehouse", new { area = "Admin", showDeleted = true });
            }

            TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
            return RedirectToAction("Index", "Warehouse", new { area = "Admin", showDeleted = true });
        }
    }
}
