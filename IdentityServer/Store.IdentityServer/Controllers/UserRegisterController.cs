using IdentityServer4.Hosting.LocalApiAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.IdentityServer.Dtos;
using Store.IdentityServer.Models;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Store.IdentityServer.Controllers
{
    [AllowAnonymous]
    //[Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRegisterController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> UserRegister(UserRegisterDto userRegisterDto)
        {
            // Email kontrolü
            var existingEmail = await _userManager.FindByEmailAsync(userRegisterDto.Email);
            if (existingEmail != null)
            {
                return BadRequest(new { Message = "Bu e-posta adresi ile daha önce kaydedilmiş bir kullanıcı var. Şifrenizi mi unuttunuz?" });
            }

            // Telefon kontrolü
            var existingPhone = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == userRegisterDto.PhoneNumber);
            if (existingPhone != null)
            {
                return BadRequest(new { Message = "Bu telefon numarası ile daha önce kaydedilmiş bir kullanıcı var. Şifrenizi mi unuttunuz?" });
            }

            var values = new ApplicationUser
            {
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                UserName = userRegisterDto.Username ?? userRegisterDto.Email,
                Email = userRegisterDto.Email,
                PhoneNumber = userRegisterDto.PhoneNumber,
                TCKimlik = userRegisterDto.TCKimlik
            };
            var result = await _userManager.CreateAsync(values, userRegisterDto.Password);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Kullanıcı başarıyla eklendi." });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}