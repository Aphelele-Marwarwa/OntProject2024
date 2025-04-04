// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly ApplicationDbContext _context; // Your application's DbContext
        private readonly BusinessService _businessService; // Add BusinessService for layout data

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            ApplicationDbContext context,
            BusinessService businessService) // Inject BusinessService
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _businessService = businessService; // Initialize BusinessService
        }

        public Employee Employee { get; set; }
        public int EmployeeId { get; set; } // Property to hold the Employee ID
        public Customer Customer { get; set; } // Property to hold the Customer object
        public int CustomerId { get; set; } // Property to hold the Customer ID

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            // Load Employee data
            Employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
            if (Employee != null)
            {
                EmployeeId = Employee.EmployeeId; // Set the Employee ID
            }

            // Load Customer data
            Customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == user.Email);
            if (Customer != null)
            {
                CustomerId = Customer.CustomerId; // Assign the CustomerId
            }

            // Call SetLayoutData method to load layout-related information (like business info, logos)
            await SetLayoutData();

            // Return the page (without errors)
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Attempt to change the user's password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // Update the Employee table password (if the user is an employee)
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
            if (employee != null)
            {
                employee.Password = Input.NewPassword; // Update password in Employee table
                _context.Employees.Update(employee); // Mark as updated
            }

            // Update the Customer table password (if the user is a customer)
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == user.Email);
            if (customer != null)
            {
                customer.Password = Input.NewPassword; // Update password in Customer table
                _context.Customers.Update(customer); // Mark as updated
            }

            // Save changes to both tables (if applicable)
            await _context.SaveChangesAsync();

            // Refresh sign-in to apply new password
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
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
