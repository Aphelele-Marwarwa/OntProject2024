// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Models.admin;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;

        private readonly ApplicationDbContext _context;
        private readonly BusinessService _businessService;

        public ResetPasswordModel(Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager, ApplicationDbContext context, BusinessService businessService)
        {
            _userManager = userManager;
            _context = context;
            _businessService = businessService;
        }

        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
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
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Code { get; set; }

        }

        public async Task<IActionResult> OnGet(string code = null)
        {
            if (code == null)
            {
                await SetLayoutData();
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                // Decode the code and set it to the Input model
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                await SetLayoutData();
                return Page();
            }
        }


            public async Task<IActionResult> OnPostAsync()
            {
                if (!ModelState.IsValid)
            {
                await SetLayoutData();
                return Page();
                }

                // Find the user by email
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                await SetLayoutData();
                return RedirectToPage("./ResetPasswordConfirmation");
                }

                // Reset the password
                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                if (result.Succeeded)
                {
                    // Find the employee record
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
                    if (employee != null)
                    {
                        // Update password in Employee table
                        employee.Password = Input.Password; // Update password in Employee table

                        // Update email if necessary
                        if (employee.Email != Input.Email)
                        {
                            employee.Email = Input.Email; // Update email in Employee table
                        }

                        // Save changes
                        await _context.SaveChangesAsync();
                    }
                     await SetLayoutData();
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                // Add any errors to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
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
