using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.SizeDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            try
            {
                ViewBag.ShowDeleted = showDeleted;
                var values = await _sizeService.GetAllSizesAsync(showDeleted);
                return View(values ?? new List<ResultSizeDto>());
            }
            catch
            {
                return View(new List<ResultSizeDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddorEdit(Guid? id = null)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
            {
                return View(new UpdateSizeDto { IsActive = true });
            }

            try
            {
                var size = await _sizeService.GetSizeByIdAsync(id.Value);
                return View(size);
            }
            catch
            {
                return View(new UpdateSizeDto { IsActive = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddorEdit(UpdateSizeDto dto)
        {
            var createDto = new CreateSizeDto
            {
                Name = dto.Name,
                SizeType = dto.SizeType,
                PrimaryValue = dto.PrimaryValue,
                SecondaryValue = dto.SecondaryValue,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001") // Geçici
            };

            var response = await _sizeService.CreateSizeAsync(createDto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var err = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", err);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSize(UpdateSizeDto updateSizeDto)
        {
            var response = await _sizeService.UpdateSizeAsync(updateSizeDto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteSize(Guid id)
        {
            var response = await _sizeService.DeleteSizeAsync(id);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Size", new { area = "Admin" });

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index", "Size", new { area = "Admin" });
        }

        public async Task<IActionResult> RestoreSize(Guid id)
        {
            var response = await _sizeService.RestoreSizeAsync(id);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Size", new { area = "Admin", showDeleted = true });

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index", "Size", new { area = "Admin", showDeleted = true });
        }
    }
}
