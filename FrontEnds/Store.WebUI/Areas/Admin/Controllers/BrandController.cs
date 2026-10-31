using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.BrandDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        private async Task LoadBrandsAsync()
        {
            try
            {
                var brands = await _brandService.GetAllBrandsAsync(false);
                ViewBag.Brands = brands ?? new List<ResultBrandDto>();
            }
            catch
            {
                ViewBag.Brands = new List<ResultBrandDto>();
            }
        }
        
        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            try
            {
                var values = await _brandService.GetAllBrandsAsync(showDeleted);
                return View(values);
            }
            catch
            {
                return View(new List<ResultBrandDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddorEdit(Guid? id = null)
        {
            await LoadBrandsAsync();
            if ((!id.HasValue || id.Value == Guid.Empty))
            {
                return View(new UpdateBrandDto { IsActive = true });
            }
            
            try
            {
                var brand = await _brandService.GetBrandByIdAsync(id.Value);
                return View(brand);
            }
            catch
            {
                return View(new UpdateBrandDto { IsActive = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddorEdit(UpdateBrandDto dto)
        {
            var createBrandDto = new CreateBrandDto 
            {
                Code = dto.Code,
                Name = dto.Name,
                Notes = dto.Notes,
                IsActive = dto.IsActive,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001") // Geçici olarak 1 veriyoruz
            };
            
            var responseMessage = await _brandService.CreateBrandAsync(createBrandDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction("Index");
            }

            var errorMessage = await responseMessage.Content.ReadAsStringAsync();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = errorMessage });
            }
            ModelState.AddModelError("", errorMessage);
            await LoadBrandsAsync();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBrand(UpdateBrandDto updateBrandDto)
        {
            var responseMessage = await _brandService.UpdateBrandAsync(updateBrandDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            var responseMessage = await _brandService.DeleteBrandAsync(id);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Brand", new { area = "Admin" });
            }

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index", "Brand", new { area = "Admin" });
        }

        public async Task<IActionResult> RestoreBrand(Guid id)
        {
            var responseMessage = await _brandService.RestoreBrandAsync(id);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Brand", new { area = "Admin", showDeleted = true });
            }

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index", "Brand", new { area = "Admin", showDeleted = true });
        }
    }
}
