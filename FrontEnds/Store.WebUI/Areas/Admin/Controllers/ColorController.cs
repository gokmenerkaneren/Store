using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.ColorDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ColorController : Controller
    {
        private readonly IColorService _colorService;

        public ColorController(IColorService colorService)
        {
            _colorService = colorService;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            try
            {
                ViewBag.ShowDeleted = showDeleted;
                var values = await _colorService.GetAllColorsAsync(showDeleted);
                return View(values ?? new List<ResultColorDto>());
            }
            catch
            {
                return View(new List<ResultColorDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddorEdit(Guid? id = null)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
            {
                return View(new UpdateColorDto { IsActive = true });
            }

            try
            {
                var color = await _colorService.GetColorByIdAsync(id.Value);
                return View(color);
            }
            catch
            {
                return View(new UpdateColorDto { IsActive = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddorEdit(UpdateColorDto dto)
        {
            // Yeni kayıt (Id boşsa Create)
            var createDto = new CreateColorDto
            {
                Name = dto.Name,
                HexCode = dto.HexCode,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001") // Geçici
            };

            var response = await _colorService.CreateColorAsync(createDto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var err = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", err);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateColor(UpdateColorDto updateColorDto)
        {
            var response = await _colorService.UpdateColorAsync(updateColorDto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteColor(Guid id)
        {
            var response = await _colorService.DeleteColorAsync(id);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Color", new { area = "Admin" });

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index", "Color", new { area = "Admin" });
        }

        public async Task<IActionResult> RestoreColor(Guid id)
        {
            var response = await _colorService.RestoreColorAsync(id);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Color", new { area = "Admin", showDeleted = true });

            var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
            TempData["ErrorMessage"] = err;
            return RedirectToAction("Index", "Color", new { area = "Admin", showDeleted = true });
        }
    }
}
