// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ResetPasswordConfirmationModel> _logger; // Updated logger type
        private readonly ApplicationDbContext _context; // Your application's DbContext
        private readonly BusinessService _businessService; // Add BusinessService for layout data

        public ResetPasswordConfirmationModel( // Fixed the constructor name
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ResetPasswordConfirmationModel> logger, // Updated logger type
            ApplicationDbContext context,
            BusinessService businessService) // Inject BusinessService
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger; // Initialize the logger
            _context = context;
            _businessService = businessService; // Initialize BusinessService
        }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            await SetLayoutData();
            return Page(); // Return the Page result to render the view
        }

        private async Task SetLayoutData()
        {
            var business = await _businessService.GetBusinessAsync();
            var setting = await _businessService.GetSettingAsync();

            ViewData["BusinessName"] = setting?.BusinessName ?? "Default Name";
            ViewData["ContactEmail"] = setting?.ContactEmail ?? "default@example.com";
            ViewData["ContactPhone"] = setting?.ContactPhone ?? "000-000-0000";
            ViewData["Address"] = $"{setting?.Business.Street}, {setting?.Business.City}, {setting?.Business.StateProvince}, {setting?.Business.Country}";
            ViewData["Slogan"] = business?.Slogan ?? "Your business slogan goes here.";

            if (setting?.BusinessLogo != null)
            {
                string businessLogoBase64 = Convert.ToBase64String(setting.BusinessLogo);
                ViewData["BusinessLogo"] = $"data:image/jpeg;base64,{businessLogoBase64}";
            }
            else
            {
                ViewData["BusinessLogo"] = "/path/to/default/logo.png"; // Provide a default logo path if needed
            }

            if (setting?.CoverPhoto != null)
            {
                string coverPhotoBase64 = Convert.ToBase64String(setting.CoverPhoto);
                ViewData["CoverPhoto"] = $"data:image/jpeg;base64,{coverPhotoBase64}";
            }
            else
            {
                ViewData["CoverPhoto"] = "/path/to/default/cover/photo.jpg"; // Provide a default cover photo path if needed
            }
        }
    }
}
