using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.admin;
using System.Text;


namespace Ont3010_Project_YA2024.Controllers
{
    public class EditProfileController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditProfileController(BusinessService businessService, ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
                                  IWebHostEnvironment webHostEnvironment, INotificationService notificationService)
                                   : base(businessService, context, notificationService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }


        private bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".tif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024; // 5 MB size limit
        }
        // GET: Employee/Edit/5

        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Employee ID is not provided.";
                await SetLayoutData();
                return RedirectToAction(nameof(EditProfile));
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction(nameof(EditProfile));
            }
            await SetLayoutData();
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditProfile(int id, Employee employee, IFormFile profilePhoto)
        {
            if (id != employee.EmployeeId)
            {
                TempData["Error"] = "Employee ID mismatch.";
                await SetLayoutData();
                return RedirectToAction(nameof(EditProfile));
            }

            if (ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors in the form.";
                await SetLayoutData();
                return View(employee); // Return to the same view with the model to display validation errors
            }

            try
            {
                var existingEmployee = await _context.Employees.FindAsync(id);
                if (existingEmployee == null)
                {
                    TempData["Error"] = "Employee not found.";
                    await SetLayoutData();
                    return RedirectToAction(nameof(EditProfile));
                }

                // Update employee properties
                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.Email = employee.Email;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.DateOfHire = employee.DateOfHire;
                existingEmployee.Role = employee.Role;
                existingEmployee.Title = employee.Title;
                existingEmployee.Responsibility = employee.Responsibility;

                // Handle profile photo upload
                if (profilePhoto != null && profilePhoto.Length > 0)
                {
                    if (IsValidImage(profilePhoto))
                    {
                        using (var ms = new MemoryStream())
                        {
                            await profilePhoto.CopyToAsync(ms);
                            existingEmployee.ProfilePhoto = ms.ToArray(); // Store as byte array
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Invalid image file.";
                        return View(employee); // Return to the same view with the model to display validation errors
                    }
                }
                await SetLayoutData();
                _context.Update(existingEmployee);
                await _context.SaveChangesAsync();
                TempData["updated"] = "Employee updated successfully!";
                return RedirectToAction(nameof(EditProfile));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.EmployeeId))
                {
                    TempData["Error"] = "Employee not found.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while updating the employee.";
                }
                await SetLayoutData();
                return RedirectToAction(nameof(EditProfile));
            }
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id && !e.IsDeleted);
        }
    }
}
