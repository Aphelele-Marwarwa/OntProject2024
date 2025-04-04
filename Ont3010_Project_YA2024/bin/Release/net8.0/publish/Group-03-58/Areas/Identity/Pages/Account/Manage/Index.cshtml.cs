// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Models.admin;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly BusinessService _businessService; // Inject BusinessService

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            BusinessService businessService) // Add BusinessService to constructor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _businessService = businessService; // Initialize BusinessService
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }


        public Employee Employee { get; set; }
        public int EmployeeId { get; set; } // Property to hold the Employee ID
        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            // Assuming there's a way to link IdentityUser to Employee
            Employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);

            if (Employee == null)
            {
                return NotFound($"Unable to load employee data for user with ID '{user.Id}'.");
            }

            EmployeeId = Employee.EmployeeId; // Set the Employee ID

            await SetLayoutData(); // Call SetLayoutData method

            return Page();
        }

        private async Task SetLayoutData()
        {
            var business = await _businessService.GetBusinessAsync();
            var setting = await _businessService.GetSettingAsync();

            ViewData["BusinessName"] = business?.LocationName ?? "Default Name";
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

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
            if (employee != null)
            {
                employee.PhoneNumber = Input.PhoneNumber; // Update phone number in Employee table
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
