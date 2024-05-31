// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using BaSys.Common.Enums;
using BaSys.Host.Abstractions;
using BaSys.Host.Identity.Models;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.Host.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<WorkDbUser> _signInManager;
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly ILoggerService _basysLogger;
        private readonly IUserSettingsService _userSettingsService;

        public LoginModel(SignInManager<WorkDbUser> signInManager,
            IDbInfoRecordsProvider dbInfoRecordsProvider,
            ILoggerService basysLogger,
            IUserSettingsService userSettingsService)
        {
            _signInManager = signInManager;
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
            _basysLogger = basysLogger;
            _userSettingsService = userSettingsService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            
            [Required]
            public string DbName { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string dbName, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            if (!string.IsNullOrEmpty(dbName))
            {
                Input = new InputModel
                {
                    DbName = dbName
                };
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/app");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(Input.DbName);
                if (dbInfoRecord?.IsDeleted == true)
                {
                    ModelState.AddModelError(string.Empty, $"Database with name '{Input.DbName}' is disabled.");
                    return Page();
                }
                
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    await SetLocalizationAsync();

                    _basysLogger.Info("User logged in", EventTypeFactory.UserLogin);
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _basysLogger.Info("User account locked out", EventTypeFactory.UserLogin);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    _basysLogger.Error("Invalid login attempt", EventTypeFactory.UserLoginFail);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task SetLocalizationAsync()
        {
            var userLanguage = Languages.English;
            var userId = _signInManager.UserManager.GetUserId(_signInManager.Context.User);
            var result = await _userSettingsService.GetUserSettings(userId);
            if (result.IsOK)
                userLanguage = result.Data.Language;

            var cultureName = userLanguage == Languages.English ? "en-US" : "ru-RU";
            var culture = CultureInfo.GetCultureInfo(cultureName);

            var defaultCookieName = CookieRequestCultureProvider.DefaultCookieName;
            var requestCulture = new RequestCulture(culture);
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

            HttpContext.Response.Cookies.Append(defaultCookieName, cookieValue);
        }
    }
}
