using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.CategoryDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Gelen resim dosyasını wwwroot/Images/UserDefault/Category/ klasörüne kaydeder.
        /// Dosya yoksa null döner.
        /// </summary>
        private async Task<string?> SaveCategoryImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var extension = Path.GetExtension(imageFile.FileName);
            var fileName = Guid.NewGuid().ToString() + extension;
            var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "UserDefault", "Category");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/Images/UserDefault/Category/" + fileName;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(false);
                ViewBag.Categories = categories?.Where(c => c.IsActive).ToList() ?? new List<ResultCategoryDto>();
            }
            catch
            {
                ViewBag.Categories = new List<ResultCategoryDto>();
            }
        }

        private async Task<IActionResult> ProcessApiResponseAsync(System.Net.Http.HttpResponseMessage responseMessage, Func<IActionResult> successRedirect, Func<Task<IActionResult>> failureView)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (responseMessage.IsSuccessStatusCode)
            {
                if (isAjax)
                    return Json(new { success = true });
                return successRedirect();
            }

            var errorMessage = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
            if (isAjax)
                return Json(new { success = false, message = errorMessage });

            TempData["ErrorMessage"] = errorMessage;
            return await failureView();
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;
            ViewBag.ShowDeleted = actualShowDeleted;
            
            try
            {
                var allCategories = await _categoryService.GetAllCategoriesAsync(true);
                ViewBag.AllCategories = allCategories ?? new List<ResultCategoryDto>();
            }
            catch
            {
                ViewBag.AllCategories = new List<ResultCategoryDto>();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadDataTables([FromBody] Store.Shared.Dtos.DataTablesRequest request, [FromQuery] bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            try
            {
                var response = await _categoryService.GetCategoriesForDataTablesAsync(request, actualShowDeleted);
                
                return Json(new {
                    draw = response.Draw,
                    recordsTotal = response.RecordsTotal,
                    recordsFiltered = response.RecordsFiltered,
                    data = response.Data,
                    error = response.Error
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddorEdit(Guid? id = null)
        {
            await LoadCategoriesAsync();
            if ((!id.HasValue || id.Value == Guid.Empty))
            {
                return View(new UpdateCategoryDto { IsActive = true });
            }

            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                return View(category);
            }
            catch
            {
                return View(new UpdateCategoryDto { IsActive = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddorEdit(UpdateCategoryDto dto, IFormFile? imageFile)
        {
            // Resim yükleme – opsiyoneldir, null gelebilir
            var savedImageUrl = await SaveCategoryImageAsync(imageFile);

            var createCategoryDto = new CreateCategoryDto
            {
                ParentId = dto.ParentId,
                Code = dto.Code,
                Name = dto.Name,
                Notes = dto.Notes,
                IsActive = dto.IsActive,
                ImageUrl = savedImageUrl,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001")
            };

            var response = await _categoryService.CreateCategoryAsync(createCategoryDto);

            return await ProcessApiResponseAsync(
                response,
                () => RedirectToAction("Index"),
                async () =>
                {
                    await LoadCategoriesAsync();
                    return View(dto);
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto, IFormFile? imageFile)
        {
            // Yeni resim geldiyse güncelle; gelmedi ise mevcut ImageUrl (hidden input ile) korunur
            var savedImageUrl = await SaveCategoryImageAsync(imageFile);
            if (savedImageUrl != null)
                updateCategoryDto.ImageUrl = savedImageUrl;

            var responseMessage = await _categoryService.UpdateCategoryAsync(updateCategoryDto);

            return await ProcessApiResponseAsync(
                responseMessage,
                () => RedirectToAction("Index"),
                () => Task.FromResult<IActionResult>(RedirectToAction("Index"))
            );
        }

        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var responseMessage = await _categoryService.DeleteCategoryAsync(id);
            return await ProcessApiResponseAsync(
                responseMessage,
                () => RedirectToAction("Index", "Category", new { area = "Admin" }),
                () => Task.FromResult<IActionResult>(RedirectToAction("Index", "Category", new { area = "Admin" }))
            );
        }

        public async Task<IActionResult> RestoreCategory(Guid id)
        {
            var responseMessage = await _categoryService.RestoreCategoryAsync(id);
            return await ProcessApiResponseAsync(
                responseMessage,
                () => RedirectToAction("Index", "Category", new { area = "Admin", showDeleted = true }),
                () => Task.FromResult<IActionResult>(RedirectToAction("Index", "Category", new { area = "Admin", showDeleted = true }))
            );
        }
    }
}
