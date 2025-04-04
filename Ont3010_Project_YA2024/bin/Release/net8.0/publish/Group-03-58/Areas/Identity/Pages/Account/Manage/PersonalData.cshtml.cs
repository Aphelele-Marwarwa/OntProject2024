using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.CustomerLiaison;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly BusinessService _businessService;

        public PersonalDataModel(
            UserManager<IdentityUser> userManager,
            ILogger<PersonalDataModel> logger,
            ApplicationDbContext context,
            BusinessService businessService)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _businessService = businessService;
        }

        public Employee Employee { get; set; }
        public Customer Customer { get; set; } // Add this property for customer data
        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Check user roles sequentially
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
            var isCustomerLiaison = await _userManager.IsInRoleAsync(user, "Customer Liaison");
            var isInventoryLiaison = await _userManager.IsInRoleAsync(user, "Inventory Liaison");
            var isPurchasingManager = await _userManager.IsInRoleAsync(user, "Purchasing Manager");
            var isCustomer = await _userManager.IsInRoleAsync(user, "Customer");
            var isFaultTechnician = await _userManager.IsInRoleAsync(user, "Fault Technician");
            var isMaintenanceTechnician = await _userManager.IsInRoleAsync(user, "Maintenance Technician");

            // Adjust access based on role
            if (!isAdmin && !isCustomerLiaison && !isInventoryLiaison && !isPurchasingManager && !isCustomer && !isFaultTechnician && !isMaintenanceTechnician)
            {
                return Forbid(); // Deny access if not in one of the allowed roles
            }

            // Load Employee data only if the user is in an employee-related role
            if (isAdmin || isCustomerLiaison || isInventoryLiaison || isPurchasingManager || isFaultTechnician || isMaintenanceTechnician)
            {
                Employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
                if (Employee == null)
                {
                    _logger.LogWarning($"Employee not found for user with email '{user.Email}'.");
                }
                else
                {
                    EmployeeId = Employee.EmployeeId; // Set the Employee ID
                }
            }

            // Load Customer data only if the user is in the 'Customer' role
            if (isCustomer)
            {
                Customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == user.Email);
                if (Customer == null)
                {
                    return NotFound($"Unable to load customer data for user with ID '{user.Id}'.");
                }
                else
                {
                    CustomerId = Customer.CustomerId; // Assign the CustomerId
                }
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
                ViewData["BusinessLogo"] = "/path/to/default/logo.png";
            }

            if (setting?.CoverPhoto != null)
            {
                string coverPhotoBase64 = Convert.ToBase64String(setting.CoverPhoto);
                ViewData["CoverPhoto"] = $"data:image/jpeg;base64,{coverPhotoBase64}";
            }
            else
            {
                ViewData["CoverPhoto"] = "/path/to/default/cover/photo.jpg";
            }
        }
    }
}