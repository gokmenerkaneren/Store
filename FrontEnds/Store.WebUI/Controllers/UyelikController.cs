using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Models;
using Store.WebUI.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Store.WebUI.Controllers
{
    public class UyelikController : Controller
    {
        private readonly IUserRegisterApiService _userRegisterApiService;
        private readonly IIdentityApiService _identityApiService;
        private readonly ILocationApiService _locationApiService;

        public UyelikController(IUserRegisterApiService userRegisterApiService, IIdentityApiService identityApiService, ILocationApiService locationApiService)
        {
            _userRegisterApiService = userRegisterApiService;
            _identityApiService = identityApiService;
            _locationApiService = locationApiService;
        }

        [HttpPost]
        public async Task<IActionResult> CheckUser([FromBody] CheckUserExistsRequest request)
        {
            var response = await _identityApiService.CheckUserExistsAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json"); // returns { "exists": true/false }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                bool isTokenValid = false;
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwt = handler.ReadJwtToken(token);
                        if (jwt.ValidTo > DateTime.UtcNow)
                        {
                            isTokenValid = true;
                        }
                    }
                }

                if (isTokenValid)
                {
                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Default");
                }
                else
                {
                    // Token süresi dolmuş veya geçersizse eski oturum çerezini temizle
                    await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                    Response.Cookies.Delete("StoreWebUIAuth");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Lütfen tüm zorunlu alanları doldurunuz." });
            }

            var response = await _userRegisterApiService.RegisterUserAsync(model);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { Message = "Kaydınız başarıyla tamamlandı. Lütfen giriş yapınız." });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest(errorContent);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string EmailOrPhone, string Password)
        {
            var data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "client_id", "StoreAdminId" },
                { "client_secret", "storesecret" },
                { "grant_type", "password" },
                { "username", EmailOrPhone },
                { "password", Password }
            };

            var response = await _identityApiService.GetTokenAsync(data);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content);
                var accessToken = tokenResponse.GetProperty("access_token").GetString();
                
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(accessToken);

                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(
                    jwtToken.Claims, 
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, 
                    "name", 
                    "role");
                
                var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = jwtToken.ValidTo
                };

                authProperties.StoreTokens(new[]
                {
                    new AuthenticationToken { Name = "access_token", Value = accessToken }
                });

                await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
                
                Response.Cookies.Delete("NewsletterSubscribed");

                var emailClaim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email) ?? claimsIdentity.FindFirst("email");
                var userEmail = emailClaim?.Value ?? EmailOrPhone;
                
                try 
                {
                    bool isSubscribed = await _locationApiService.CheckBultenAboneligiAsync(userEmail);
                    if (isSubscribed)
                    {
                        Response.Cookies.Append("NewsletterSubscribed", "true", new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddYears(1)
                        });
                    }
                } 
                catch (Exception) 
                {
                    // Ignore API errors
                }

                return RedirectToAction("Index", "Default");
            }

            TempData["LoginError"] = "Şifreniz hatalı. Lütfen tekrar deneyiniz.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("NewsletterSubscribed");
            Response.Cookies.Delete("StoreWebUIAuth");
            return RedirectToAction("Index", "Default");
        }

        [HttpGet]
        public IActionResult ExternalLoginCallback()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLoginCallback(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["LoginError"] = "Google ile giriş başarısız oldu (Token alınamadı).";
                return RedirectToAction("Index");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(accessToken);

                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(
                    jwtToken.Claims, 
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, 
                    "name", 
                    "role");
                
                var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = jwtToken.ValidTo
                };

                authProperties.StoreTokens(new[]
                {
                    new AuthenticationToken { Name = "access_token", Value = accessToken }
                });

                await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
                
                Response.Cookies.Delete("NewsletterSubscribed");

                var emailClaim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email) ?? claimsIdentity.FindFirst("email");
                if (emailClaim != null)
                {
                    try 
                    {
                        bool isSubscribed = await _locationApiService.CheckBultenAboneligiAsync(emailClaim.Value);
                        if (isSubscribed)
                        {
                            Response.Cookies.Append("NewsletterSubscribed", "true", new Microsoft.AspNetCore.Http.CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(1)
                            });
                        }
                    } 
                    catch (Exception) {}
                }

                return RedirectToAction("Index", "Default");
            }
            catch
            {
                TempData["LoginError"] = "Google ile giriş başarısız oldu (Geçersiz token).";
                return RedirectToAction("Index");
            }
        }

        // [DEVELOPER TOOLS] Swagger testi için token'ı ekrana basan yardımcı metot
        [HttpGet]
        public async Task<IActionResult> ShowMyToken()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            return Content(token ?? "Token bulunamadı. Lütfen önce giriş yapın.");
        }
    }
}
