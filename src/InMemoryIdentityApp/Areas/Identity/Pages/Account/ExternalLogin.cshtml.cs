﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using InMemoryIdentityApp.Constants;
using InMemoryIdentityApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using InMemoryIdentityApp.Extensions;
using InMemoryIdentityApp.Data;
using BazorAuth.Shared;
using Microsoft.AspNetCore.Http;

namespace InMemoryIdentityApp.Areas.Identity.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly List<OpenIdConnectSchemeRecord> _oidcConfigRecords;
        private readonly ILogger<ExternalLoginModel> _logger;
        private string[] _possibleNameTypes = new[] {  ClaimTypes.Name, ClaimTypes.GivenName, ClaimTypes.Email, "DisplayName", "preferred_username", "name" };

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            List<OpenIdConnectSchemeRecord> oidcConfigRecords,
            ILogger<ExternalLoginModel> logger
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _oidcConfigRecords = oidcConfigRecords;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }
        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            Request.HttpContext.Items.Add("a", "b");
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            string currentNameIdClaimValue = null;
            if (User.Identity.IsAuthenticated)
            {
                // we will only create a new user if the user here is actually new.
                var qName = from claim in User.Claims
                            where claim.Type == ".nameIdentifier"
                            select claim;
                var nc = qName.FirstOrDefault();
                currentNameIdClaimValue = nc?.Value;
            }

            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var oidcConfig = (from item in _oidcConfigRecords
                                  where item.Scheme == info.LoginProvider
                                  select item).FirstOrDefault();
            if (oidcConfig == null)
            {
                throw new Exception($"Error getting oidcConfig for loginProvider:{info.LoginProvider}");
            }
            var oidc = await HarvestOidcDataAsync();
            HttpContext.Session.Set(Wellknown.OIDCSessionKey, new OpenIdConnectSessionDetails
            {
                LoginProider = info.LoginProvider,
                OIDC = oidc
            });
            var r = HttpContext.Session.Get<OpenIdConnectSessionDetails>(Wellknown.OIDCSessionKey);

            var queryNameId = from claim in info.Principal.Claims
                              where claim.Type == ClaimTypes.NameIdentifier
                              select claim;
            var nameIdClaim = queryNameId.FirstOrDefault();
            var displayName = nameIdClaim.Value;

            var query = from claim in info.Principal.Claims
                        where oidcConfig.DisplayNameClaimName == claim.Type
                        select claim;
            var nameClaim = query.FirstOrDefault();
            if (nameClaim != null)
            {
                displayName = nameClaim.Value;
            }
            if (currentNameIdClaimValue == nameIdClaim.Value)
            {

                // this is a re login from the same user, so don't do anything;
                return LocalRedirect(returnUrl);
            }
            /*
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);

            if (result.Succeeded)
            {
                // Update the token
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
             */
            var queryEmail = from claim in info.Principal.Claims
                              where claim.Type == ClaimTypes.Email
                              select claim;
            var emailClaim = queryNameId.FirstOrDefault();
            var email = emailClaim?.Value;
            var leftoverUser = await _userManager.FindByEmailAsync(displayName);
            if (leftoverUser != null)
            {
                await _userManager.DeleteAsync(leftoverUser); // just using this inMemory userstore as a scratch holding pad
            }
            var user = new ApplicationUser { DisplayName = displayName, UserName = nameIdClaim.Value, Email = email };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                var newUser = await _userManager.FindByIdAsync(user.Id);
                var eClaims = new List<Claim>
                {
                    new Claim("display-name", displayName),
                    new Claim("login_provider",info.LoginProvider)
                };
                // normalized id.
                await _userManager.AddClaimsAsync(newUser, eClaims);

                await _signInManager.SignInAsync(user, isPersistent: false);
                await _userManager.DeleteAsync(user); // just using this inMemory userstore as a scratch holding pad
                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                return LocalRedirect(returnUrl);
            }
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
        private async Task<Dictionary<string, string>> HarvestOidcDataAsync()
        {
            var at = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "access_token");
            var idt = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "id_token");
            var rt = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "refresh_token");
            var tt = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "token_type");
            var ea = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "expires_at");

            var oidc = new Dictionary<string, string>
            {
                {"access_token", at},
                {"id_token", idt},
                {"refresh_token", rt},
                {"token_type", tt},
                {"expires_at", ea}
            };
            return oidc;
        }
    }
}
