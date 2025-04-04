// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly BusinessService _businessService;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger,
            BusinessService businessService,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _businessService = businessService;
            _context = context;
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

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to the home page if the user is already authenticated
                Response.Redirect("/");
                return; // Ensure the method exits after redirecting
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            await SetLayoutData();
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
                ViewData["CoverPhoto"] = "/images/adminIcons/Profile.png"; // Provide a default cover photo path if needed
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Fetch the user by email first
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    await SetLayoutData();
                    return Page();
                }

                // Fetch the employee details using the user's email
                var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Email == Input.Email);

                // Fetch the customer details using the user's email
                var customer = await _context.Customers.SingleOrDefaultAsync(c => c.EmailAddress == Input.Email);

                // Check if the employee exists and is active, or if the customer exists and is active
                if ((employee == null || employee.IsDeleted) && (customer == null || customer.IsDeleted))
                {
                    ModelState.AddModelError(string.Empty, "This account is no longer active.");
                    await SetLayoutData();
                    return Page();
                }

                // Proceed with sign-in attempt for active users (either employee or customer)
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, isPersistent: Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Fetch the user's roles
                    var roles = await _userManager.GetRolesAsync(user);

                    // Redirect based on roles (add more roles if necessary)
                    if (roles.Contains("Administrator"))
                    {
                        return LocalRedirect("/Admin");
                    }
                    else if (roles.Contains("Customer Liaison"))
                    {
                        return LocalRedirect("/CustomerLiaison");
                    }
                    else if (roles.Contains("Inventory Liaison"))
                    {
                        return LocalRedirect("/InventoryLiaison");
                    }
                    else if (roles.Contains("Customer"))
                    {
                        return LocalRedirect("/CustomerCon");
                    }
                    else if (roles.Contains("Fault Technician"))
                    {
                        return LocalRedirect("/FaultTechnician");
                    }
                    else if (roles.Contains("Maintenance Technician"))
                    {
                        return LocalRedirect("/MaintenanceTechnician");
                    }
                    else if (roles.Contains("Purchasing Manager"))
                    {
                        return LocalRedirect("/PurchasingManager");
                    }
                    else if (roles.Contains("Supplier"))
                    {
                        return LocalRedirect("/Supplier");
                    }
                    else
                    {
                        return LocalRedirect(returnUrl);
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    await SetLayoutData();
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            await SetLayoutData();
            return Page();
        }

    }
}
