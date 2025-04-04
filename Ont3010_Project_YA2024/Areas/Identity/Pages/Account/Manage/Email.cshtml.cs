// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly BusinessService _businessService; // Inject BusinessService

        public EmailModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender emailSender,
            ApplicationDbContext context,
            BusinessService businessService) // Add BusinessService to constructor
        {
            _userManager = userManager;
            _signInManager = signInManager; 
            _emailSender = emailSender;
            _context = context;
            _businessService = businessService; // Initialize BusinessService
        }

        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }

        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            // Try to load Employee data
            Employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
            if (Employee != null)
            {
                EmployeeId = Employee.EmployeeId; // Set the Employee ID if found
            }

            // Try to load Customer data
            Customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == user.Email);
            if (Customer != null)
            {
                CustomerId = Customer.CustomerId; // Set the Customer ID if found
            }

            // Handle the case where neither employee nor customer data is found
            if (Employee == null && Customer == null)
            {
                return NotFound($"Neither employee nor customer data found for user with ID '{user.Id}'.");
            }

            await SetLayoutData(); // Call SetLayoutData method

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

        public async Task<IActionResult> OnPostChangeEmailAsync()
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

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                // Check if the new email is already in use
                var existingUser = await _userManager.FindByEmailAsync(Input.NewEmail);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already in use.");
                    await LoadAsync(user);
                    return Page();
                }

                // Generate the email change token
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);

                // Update the employee's email only if it exists
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
                if (employee != null)
                {
                    employee.Email = Input.NewEmail; // Update the employee's email
                    await _context.SaveChangesAsync();
                }

                // Send the confirmation email
                await _emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);

            // Generate email confirmation token
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            // Update email in Employee table if employee exists
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (employee != null)
            {
                employee.Email = Input.NewEmail; // Update employee email
                _context.Employees.Update(employee); // Mark as updated
            }

            // Update email in Customer table if customer exists
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == email);
            if (customer != null)
            {
                customer.EmailAddress = Input.NewEmail; // Update customer email
                _context.Customers.Update(customer); // Mark as updated
            }
            else if (employee == null)
            {
                // If neither employee nor customer is found, return an error
                return NotFound($"Unable to load customer or employee data for user with ID '{user.Id}'.");
            }

            // Save changes to both tables (Employee and Customer)
            await _context.SaveChangesAsync();

            // Send the verification email
            await _emailSender.SendEmailAsync(
                Input.NewEmail, // Send to the new email address
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }



    }
}
