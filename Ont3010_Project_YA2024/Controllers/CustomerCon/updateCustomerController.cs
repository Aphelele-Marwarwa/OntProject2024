using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.CustomerCon
{
    public class updateCustomerController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;

        public updateCustomerController(BusinessService businessService, ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _context = context;
            _userManager = userManager;
        }
        private bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".tif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024; // 5 MB size limit
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound("Customer ID is not provided.");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(e => e.CustomerId == id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

          
            await SetLayoutData(); // Ensure layout data is set before rendering the view
            await CustomerNotification();
            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer, IFormFile profilePhoto)
        {
            if (id != customer.CustomerId)
            {
                return NotFound("Customer ID mismatch.");
            }

            // Check model state before proceeding
            if (!ModelState.IsValid)
            {
                // Check for existing email
                if (EmailExists(customer.EmailAddress, id))
                {
                    TempData["Error"] = "A customer with this email already exists.";
                  
                    await SetLayoutData();
                    await CustomerNotification();
                    return View(customer);
                }

                // Update only necessary fields, ignore password and created fields
                try
                {
                    var existingCustomer = await _context.Customers.FindAsync(id);
                    if (existingCustomer == null)
                    {
                        return NotFound("Customer not found.");
                    }

                    // Update fields excluding password, CreatedBy, and CreatedDate
                    existingCustomer.FirstName = customer.FirstName;
                    existingCustomer.LastName = customer.LastName;
                    existingCustomer.Title = customer.Title;
                    existingCustomer.EmailAddress = customer.EmailAddress;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    existingCustomer.BusinessRole = customer.BusinessRole;
                    existingCustomer.StreetAddress = customer.StreetAddress;
                    existingCustomer.City = customer.City;
                    existingCustomer.PostalCode = customer.PostalCode;
                    existingCustomer.Province = customer.Province;
                    existingCustomer.Country = customer.Country;

                    // Update business-specific fields
                    existingCustomer.BusinessName = customer.BusinessName;
                    existingCustomer.BusinessEmailAddress = customer.BusinessEmailAddress;
                    existingCustomer.BusinessPhoneNumber = customer.BusinessPhoneNumber;
                    existingCustomer.BusinessType = customer.BusinessType;
                    existingCustomer.Industry = customer.Industry;
                    existingCustomer.ModifiedBy = User.Identity.Name;
                    existingCustomer.ModifiedDate = DateTime.Now;
                    if (profilePhoto != null && profilePhoto.Length > 0)
                    {
                        if (IsValidImage(profilePhoto))
                        {
                            using (var ms = new MemoryStream())
                            {
                                await profilePhoto.CopyToAsync(ms);
                                existingCustomer.ProfilePhoto = ms.ToArray(); // Store as byte array
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Invalid image file.";
                            return RedirectToAction(nameof(Edit)); 
                        }
                    }
                    _context.Customers.Update(existingCustomer);
                    await _context.SaveChangesAsync();
                    TempData["updated"] = "Customer updated successfully!";
                    return RedirectToAction(nameof(Edit));
                }
                catch (DbUpdateConcurrencyException)
                {
                  
                        throw; // Handle concurrency issue
                    
                }
            }

            // If we got this far, something failed; redisplay form
          
            await SetLayoutData(); // Ensure layout data is set before rendering the view with validation errors
            await CustomerNotification();
            return View(customer);
        }

        private bool EmailExists(string email, int? customerId = null)
        {
            if (customerId == null)
            {
                return _context.Customers.Any(c => c.EmailAddress == email);
            }
            return _context.Customers.Any(c => c.EmailAddress == email && c.CustomerId != customerId);
        }
    }
}
