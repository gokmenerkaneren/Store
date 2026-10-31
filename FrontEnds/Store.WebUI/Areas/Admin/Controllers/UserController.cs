using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.IdentityDtos.RoleDtos;
using Store.DtoLayer.IdentityDtos.UserDtos;
using Store.DtoLayer.LocationDtos.UserAddressDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = "Admin,SuperAdmin")] // In production, limit access
    [Authorize]
    public class UserController : Controller
    {
        private readonly IIdentityApiService _identityApiService;
        private readonly ILocationApiService _locationApiService;

        public UserController(IIdentityApiService identityApiService, ILocationApiService locationApiService)
        {
            _identityApiService = identityApiService;
            _locationApiService = locationApiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _identityApiService.GetAllUsersAsync();
                return View(users ?? new List<ResultUserDto>());
            }
            catch
            {
                return View(new List<ResultUserDto>());
            }
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return RedirectToAction("CreateOrUpdateUser");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            try
            {
                var response = await _identityApiService.CreateUserAsync(createUserDto);
                if (response.IsSuccessStatusCode)
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    var jObj = System.Text.Json.JsonDocument.Parse(contentString);
                    var userId = jObj.RootElement.GetProperty("id").GetString();
                    return Json(new { success = true, userId = userId });
                }
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateUser(Guid? id = null)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
            {
                ViewBag.IsUpdate = false;
                ViewBag.UserId = Guid.Empty;
                return View(new UpdateUserDto());
            }

            ViewBag.IsUpdate = true;
            ViewBag.UserId = id.Value;

            try
            {
                var user = await _identityApiService.GetUserByIdAsync(id.Value);
                var updateDto = new UpdateUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TCKimlik = user.TCKimlik,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                // Get Roles
                try
                {
                    ViewBag.AllRoles = await _identityApiService.GetAllRolesAsync();
                    ViewBag.UserRoles = await _identityApiService.GetUserRolesAsync(id.Value);
                }
                catch
                {
                    ViewBag.AllRoles = new List<ResultRoleDto>();
                    ViewBag.UserRoles = new List<string>();
                }

                return View(updateDto);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            try
            {
                var response = await _identityApiService.UpdateUserAsync(updateUserDto);
                if (response.IsSuccessStatusCode) return Json(new { success = true });
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var responseMessage = await _identityApiService.DeleteUserAsync(id);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        // Roles

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleDto assignRoleDto)
        {
            try
            {
                var response = await _identityApiService.AssignRoleAsync(assignRoleDto);
                if (response.IsSuccessStatusCode) return Json(new { success = true });
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Addresses

        [HttpGet]
        public async Task<IActionResult> GetAddressesForUser(Guid userId)
        {
            try
            {
                // In a real scenario, GetUserAddressesAsync would take a userId param
                // But for now let's just get all and filter, or assume the API will be updated
                var allAddresses = await _locationApiService.GetUserAddressesAsync(false);
                var userAddresses = allAddresses.FindAll(x => x.CustomerId == userId);

                return Json(userAddresses);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(CreateUserAddressDto createDto)
        {
            try
            {
                var response = await _locationApiService.CreateUserAddressAsync(createDto);
                if (response.IsSuccessStatusCode) return Json(new { success = true });
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            try
            {
                var response = await _locationApiService.DeleteUserAddressAsync(id);
                if (response.IsSuccessStatusCode) return Json(new { success = true });
                return Json(new { success = false, message = "Adres silinemedi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
