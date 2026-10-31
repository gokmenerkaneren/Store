using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.DefaultSlider1Dtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DefaultSliderController : Controller
    {
        private readonly IDefaultSlider1Service _service;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DefaultSliderController(IDefaultSlider1Service service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<string?> SaveSliderImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var extension = Path.GetExtension(imageFile.FileName);
            var fileName = Guid.NewGuid().ToString() + extension;
            var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "UserDefault", "DefaultSlider1");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/Images/UserDefault/DefaultSlider1/" + fileName;
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

        public IActionResult Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;
            ViewBag.ShowDeleted = actualShowDeleted;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadDataTables([FromBody] Store.Shared.Dtos.DataTablesRequest request, [FromQuery] bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;

            try
            {
                var response = await _service.GetForDataTablesAsync(request, actualShowDeleted);

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
            if (!id.HasValue || id.Value == Guid.Empty)
            {
                return View(new UpdateDefaultSlider1Dto { IsActive = true, Sirano = 1 });
            }

            try
            {
                var slider = await _service.GetByIdAsync(id.Value);
                return View(slider);
            }
            catch
            {
                return View(new UpdateDefaultSlider1Dto { IsActive = true, Sirano = 1 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddorEdit(UpdateDefaultSlider1Dto dto, IFormFile? imageFile)
        {
            var savedImageUrl = await SaveSliderImageAsync(imageFile);

            var createDto = new CreateDefaultSlider1Dto
            {
                Sirano = dto.Sirano,
                Baslik = dto.Baslik,
                KisaAciklama = dto.KisaAciklama,
                IsActive = dto.IsActive,
                ResimYolu = savedImageUrl,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001")
            };

            var response = await _service.CreateAsync(createDto);

            return await ProcessApiResponseAsync(
                response,
                () => RedirectToAction("Index"),
                async () => View(dto)
            );
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSlider(UpdateDefaultSlider1Dto dto, IFormFile? imageFile)
        {
            var savedImageUrl = await SaveSliderImageAsync(imageFile);
            if (savedImageUrl != null)
                dto.ResimYolu = savedImageUrl;

            var responseMessage = await _service.UpdateAsync(dto);

            return await ProcessApiResponseAsync(
                responseMessage,
                () => RedirectToAction("Index"),
                () => Task.FromResult<IActionResult>(RedirectToAction("Index"))
            );
        }

        public async Task<IActionResult> DeleteSlider(Guid id)
        {
            var responseMessage = await _service.DeleteAsync(id);
            return await ProcessApiResponseAsync(
                responseMessage,
                () => RedirectToAction("Index", "DefaultSlider", new { area = "Admin" }),
                () => Task.FromResult<IActionResult>(RedirectToAction("Index", "DefaultSlider", new { area = "Admin" }))
            );
        }

        public async Task<IActionResult> RestoreSlider(Guid id)
        {
            var responseMessage = await _service.RestoreAsync(id);
            return await ProcessApiResponseAsync(
                responseMessage,
                () => RedirectToAction("Index", "DefaultSlider", new { area = "Admin", showDeleted = true }),
                () => Task.FromResult<IActionResult>(RedirectToAction("Index", "DefaultSlider", new { area = "Admin", showDeleted = true }))
            );
        }
    }
}
