// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using BaSys.Common.Enums;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.Identity.Models;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.Translation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.Host.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMainConnectionFactory _connectionFactory;
        private readonly ILoggerService _basysLogger;

        public LoginModel(IServiceProvider serviceProvider, 
            ILoggerService basysLogger, 
            IMainConnectionFactory connectionFactory)
        {
            _serviceProvider = serviceProvider;

            _connectionFactory = connectionFactory;
            _basysLogger = basysLogger;
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

            var signInManager = _serviceProvider.GetService<SignInManager<WorkDbUser>>();
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            Input = new InputModel
            {
                RememberMe = true,
            };
            if (!string.IsNullOrEmpty(dbName))
            {
                Input.DbName = dbName;
            }

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl ??= Url.Content("~/app");
            var dbInfoRecordsProvider = _serviceProvider.GetService<IDbInfoRecordsProvider>();
            var dbInfoRecord = dbInfoRecordsProvider.GetDbInfoRecordByDbName(Input.DbName);

            if (dbInfoRecord == null)
            {
                ModelState.AddModelError(string.Empty, $"\"{Input.DbName}\" - {DictMain.DataBaseNotFound}.");
                return Page();
            }

            if (dbInfoRecord.IsDeleted)
            {
                ModelState.AddModelError(string.Empty, $"\"{Input.DbName}\" - {DictMain.DataBaseDisabled}.");
                return Page();
            }

            var signInManager = _serviceProvider.GetService<SignInManager<WorkDbUser>>();
            var userSettingsService = _serviceProvider.GetService<IUserSettingsService>();


            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {

                await SetLocalizationAsync(_connectionFactory, userSettingsService, signInManager);

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

        private async Task SetLocalizationAsync(IMainConnectionFactory connectionFactory,
            IUserSettingsService userSettingsService,
            SignInManager<WorkDbUser> signInManager)
        {
            var userLanguage = Languages.English;
            var userId = signInManager.UserManager.GetUserId(signInManager.Context.User);
            using (IDbConnection connection = connectionFactory.CreateConnection())
            {
                userSettingsService.SetUp(connection);
                var result = await userSettingsService.GetUserSettings(userId);
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
}
