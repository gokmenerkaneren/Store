using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.IdentityServer.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.IdentityServer.Controllers
{
    [Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.FirstName,
                u.LastName,
                u.TCKimlik,
                u.Email,
                u.PhoneNumber,
                IsActive = u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow
            }).ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.TCKimlik,
                user.Email,
                user.PhoneNumber,
                IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow
            });
        }

        [HttpPost("check-exists")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUserExists([FromBody] Dtos.CheckUserExistsRequest request)
        {
            if (string.IsNullOrEmpty(request.EmailOrPhone)) return BadRequest("E-posta veya telefon boş olamaz.");

            var userByEmail = await _userManager.FindByEmailAsync(request.EmailOrPhone);
            if (userByEmail != null) return Ok(new { exists = true });

            var userByPhone = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.EmailOrPhone);
            if (userByPhone != null) return Ok(new { exists = true });

            return Ok(new { exists = false });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] Dtos.UserCreateRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                TCKimlik = request.TCKimlik,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return Ok(new { user.Id });
            }

            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] Dtos.UserUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(request.UserName))
                user.UserName = request.UserName;
            if (!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName;
            if (!string.IsNullOrWhiteSpace(request.TCKimlik))
                user.TCKimlik = request.TCKimlik;
            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors.Select(x => x.Description));

            if (!string.IsNullOrEmpty(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!passResult.Succeeded) return BadRequest(passResult.Errors.Select(x => x.Description));
            }

            return Ok("Kullanıcı güncellendi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded) return Ok("Kullanıcı silindi.");

            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToListAsync();
            return Ok(roles);
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            return Ok(userRoles);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] Dtos.UserAssignRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (request.RoleNames != null && request.RoleNames.Any())
            {
                await _userManager.AddToRolesAsync(user, request.RoleNames);
            }

            return Ok("Roller güncellendi.");
        }
    }
}