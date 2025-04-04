using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.admin;
using System.Linq;
using System.Threading.Tasks;

namespace Ont3010_Project_YA2024.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    public class BusinessController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BusinessController(BusinessService businessService, ApplicationDbContext context, INotificationService notificationService)
                   : base(businessService, context, notificationService) // Pass both required parameters to the base constructor
        {
            _context = context;
        }


        // GET: Business/Edit
        // GET: Business/Edit
        public async Task<IActionResult> Edit()
        {
            await EmployeeNotification();
            await SetLayoutData();
            // Log or debug here to ensure the method is called
            var business = await _context.Businesses.FirstOrDefaultAsync(b => b.BusinessId == 1);
            if (business == null)
            {
                business = new Business
                {
                    LocationName = "Default Location",
                    Slogan = "Default Slogan",
                    Street = "Default Street",
                    City = "Default City",
                    PostalCode = "0000",
                    StateProvince = "Default State",
                    Country = "Default Country",
                    ContactPerson = "Default Person",
                    ContactEmail = "default@example.com",
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };
                _context.Businesses.Add(business);
                await _context.SaveChangesAsync();
            }
            return View(business);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Business business)
        {
            if (business.BusinessId != 1)
            {
                return NotFound("Business ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var existingBusiness = await _context.Businesses.FindAsync(business.BusinessId);
                    if (existingBusiness == null)
                    {
                        return NotFound("Business not found.");
                    }

                    // Update the audit fields
                    existingBusiness.LocationName = business.LocationName;
                    existingBusiness.Slogan = business.Slogan;
                    existingBusiness.Street = business.Street;
                    existingBusiness.City = business.City;
                    existingBusiness.PostalCode = business.PostalCode;
                    existingBusiness.StateProvince = business.StateProvince;
                    existingBusiness.Country = business.Country;
                    existingBusiness.ContactPerson = business.ContactPerson;
                    existingBusiness.ContactEmail = business.ContactEmail;
                    existingBusiness.ModifiedDate = DateTime.Now;
                    existingBusiness.ModifiedBy = User.Identity.Name; // Set modified by user

                    _context.Businesses.Update(existingBusiness);
                    await _context.SaveChangesAsync();

                    TempData["updated"] = "Business details updated successfully!";
                    return RedirectToAction(nameof(Edit));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessExists(business.BusinessId))
                    {
                        return NotFound("Business not found.");
                    }
                    else
                    {
                        TempData["Error"] = "An error occurred while updating the business details. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
                }
            }
            await SetLayoutData();
            return View(business);
        }

        private bool BusinessExists(int id)
        {
            return _context.Businesses.Any(e => e.BusinessId == id);
        }
    }
}
