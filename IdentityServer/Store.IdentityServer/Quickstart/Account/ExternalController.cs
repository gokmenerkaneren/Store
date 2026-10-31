using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Store.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly ILogger<ExternalController> _logger;

        public ExternalController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILogger<ExternalController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult Challenge(string scheme, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
            
            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)), 
                Items =
                {
                    { "returnUrl", returnUrl }, 
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);
            
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);
            if (user == null)
            {
                // Instead of auto-provisioning, redirect to a completion screen
                return RedirectToAction(nameof(RegisterExternal), new { returnUrl });
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);
            
            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);
            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id.ToString();
            
            var isuser = new IdentityServerUser(user.Id.ToString())
            {
                DisplayName = name,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), name, true, context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", returnUrl);
                }
            }

            return Redirect(returnUrl);
        }

        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterExternal(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                return RedirectToAction(nameof(Callback));
            }

            var externalUser = result.Principal;
            var claims = externalUser.Claims.ToList();

            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
                        claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                        
            var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                        claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                        
            var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                       claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

            var vm = new ExternalRegisterViewModel
            {
                Email = email,
                FirstName = first,
                LastName = last,
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterExternal(ExternalRegisterViewModel model, string button)
        {
            if (button == "cancel")
            {
                var returnUrl = model.ReturnUrl ?? "~/";
                var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
                if (context != null)
                {
                    await _interaction.DenyAuthorizationAsync(context, IdentityServer4.Models.AuthorizationError.AccessDenied);
                    if (context.IsNativeClient())
                    {
                        return this.LoadingPage("Redirect", returnUrl);
                    }
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("~/");
                }
            }

            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                return RedirectToAction(nameof(Callback));
            }

            if (ModelState.IsValid)
            {
                var externalUser = result.Principal;
                var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                  externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                  throw new Exception("Unknown userid");

                var provider = result.Properties.Items["scheme"];
                var providerUserId = userIdClaim.Value;

                var emailVerifiedClaim = externalUser.FindFirst("email_verified")?.Value;
                bool emailVerified = emailVerifiedClaim == "true";

                // Check if user already exists with this email (handle duplicates gracefully by picking the first one)
                var existingUser = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
                if (existingUser != null)
                {
                    // Link external login to existing user
                    var addLoginResult = await _userManager.AddLoginAsync(existingUser, new UserLoginInfo(provider, providerUserId, provider));
                    if (addLoginResult.Succeeded)
                    {
                        var additionalLocalClaims = new List<Claim>();
                        var localSignInProps = new AuthenticationProperties();
                        ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

                        var principal = await _signInManager.CreateUserPrincipalAsync(existingUser);
                        additionalLocalClaims.AddRange(principal.Claims);
                        var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? existingUser.Id.ToString();

                        var isuser = new IdentityServerUser(existingUser.Id.ToString())
                        {
                            DisplayName = name,
                            IdentityProvider = provider,
                            AdditionalClaims = additionalLocalClaims
                        };

                        await HttpContext.SignInAsync(isuser, localSignInProps);
                        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

                        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, existingUser.Id.ToString(), name, true, context?.Client.ClientId));

                        if (context != null && context.IsNativeClient())
                        {
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        return Redirect(model.ReturnUrl ?? "~/");
                    }
                    foreach (var error in addLoginResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email, // Using email as username
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        TCKimlik = model.TCKimlik,
                        EmailConfirmed = emailVerified
                    };

                    var identityResult = await _userManager.CreateAsync(user);
                    if (identityResult.Succeeded)
                    {
                        // Add external login mapping
                        identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
                    if (identityResult.Succeeded)
                    {
                        var additionalLocalClaims = new List<Claim>();
                        var localSignInProps = new AuthenticationProperties();
                        ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

                        var principal = await _signInManager.CreateUserPrincipalAsync(user);
                        additionalLocalClaims.AddRange(principal.Claims);
                        var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id.ToString();

                        var isuser = new IdentityServerUser(user.Id.ToString())
                        {
                            DisplayName = name,
                            IdentityProvider = provider,
                            AdditionalClaims = additionalLocalClaims
                        };

                        await HttpContext.SignInAsync(isuser, localSignInProps);
                        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

                        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), name, true, context?.Client.ClientId));

                        if (context != null && context.IsNativeClient())
                        {
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        return Redirect(model.ReturnUrl ?? "~/");
                    }
                } // End if (identityResult.Succeeded) outer
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            } // End of else block (if existing user)
            } // End of if (ModelState.IsValid)

            return View(model);
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }
    }
}