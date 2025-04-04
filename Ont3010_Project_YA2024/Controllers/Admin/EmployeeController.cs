using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.admin;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;


namespace Ont3010_Project_YA2024.Controllers.Admin
{
    // [Authorize(Roles = "Administrator")]
    public class EmployeeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _emailSender;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(BusinessService businessService, ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
                                  IWebHostEnvironment webHostEnvironment, Microsoft.AspNetCore.Identity.UI.Services.IEmailSender emailSender, INotificationService notificationService)
                                   : base(businessService, context, notificationService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }


        private bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".tif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024; // 5 MB size limit
        }
        // GET: Employee Index


        public async Task<IActionResult> Index(string searchString, string sortOrder, string sortDirection, int page = 1)
        {
            await SetLayoutData();

            await EmployeeNotification();

            // Set default values
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "asc" : sortDirection;
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "FirstName" : sortOrder;

            // Set ViewData
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentDirection"] = sortDirection;
            ViewData["CurrentFilter"] = searchString;

            // Query setup
            var employees = _context.Employees.AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchTerms = searchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                employees = employees.Where(e =>
                    e.FirstName.Contains(searchString) ||
                    e.LastName.Contains(searchString) ||
                    (searchTerms.Length > 1 &&
                     searchTerms.Any(term => e.FirstName.Contains(term)) &&
                     searchTerms.Any(term => e.LastName.Contains(term)))
                );
            }

            // Apply sorting
            employees = SortEmployees(employees, sortOrder, sortDirection);

            // Pagination
            int pageSize = 10;
            var totalEmployees = await employees.CountAsync();
            var pagedList = await employees
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedList);
        }

        private IQueryable<Employee> SortEmployees(IQueryable<Employee> employees, string sortOrder, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            return sortOrder.ToLower() switch
            {
                "firstname" => isAscending ? employees.OrderBy(e => e.FirstName) : employees.OrderByDescending(e => e.FirstName),
                "lastname" => isAscending ? employees.OrderBy(e => e.LastName) : employees.OrderByDescending(e => e.LastName),
                "role" => isAscending ? employees.OrderBy(e => e.Role) : employees.OrderByDescending(e => e.Role),
                "dateofhire" => isAscending ? employees.OrderBy(e => e.DateOfHire) : employees.OrderByDescending(e => e.DateOfHire),
                _ => isAscending ? employees.OrderBy(e => e.FirstName) : employees.OrderByDescending(e => e.FirstName)
            };
        }


        public async Task<IActionResult> MarkAsRead(int id)
            {
                var userEmail = User.Identity.Name;

                    // Retrieve the employee by their email
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

                    // Find the notification status for this employee and notification
                    var status = await _context.EmployeeNotificationStatuses
                        .FirstOrDefaultAsync(s => s.NotificationId == id && s.EmployeeId == employee.EmployeeId);

                if (status != null && !status.IsRead)
                {
                    status.IsRead = true;
                    _context.Update(status);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }




        // GET: Employee Create

        public async Task<IActionResult> Create()
        {
            await SetLayoutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile profilePhoto)
        {
            if (!ModelState.IsValid)
            {
                // Check if email already exists
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == employee.Email && !e.IsDeleted);

                if (existingEmployee != null)
                {
                    ModelState.AddModelError("Email", "An employee with this email address already exists.");
                    TempData["Error"] = "Please ensure all required fields are filled out correctly.";
                    await SetLayoutData();
                    return View(employee);
                }

                try
                {
                    var generatedPassword = GenerateRandomPassword();
                    var setting = await _businessService.GetSettingAsync();
                    var business = await _businessService.GetBusinessAsync();
                    string businessName = setting?.BusinessName ?? "Default Company Name";

                    var hireDate = DateTime.Now;

                    var newEmployee = new Employee
                    {
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber,
                        DateOfHire = hireDate,
                        Role = employee.Role,
                        Title = employee.Title,
                        Responsibility = employee.Responsibility,
                        Password = generatedPassword,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name ?? "System",
                        IsDeleted = false
                    };

                    if (profilePhoto != null && profilePhoto.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await profilePhoto.CopyToAsync(memoryStream);
                            newEmployee.ProfilePhoto = memoryStream.ToArray();
                        }
                    }

                    var identityUser = new IdentityUser
                    {
                        UserName = employee.Email,
                        Email = employee.Email
                    };

                    var result = await _userManager.CreateAsync(identityUser, generatedPassword);

                    if (result.Succeeded)
                    {
                        await _userManager.ConfirmEmailAsync(identityUser, await _userManager.GenerateEmailConfirmationTokenAsync(identityUser));

                        await _userManager.AddToRoleAsync(identityUser, employee.Role);
                        _context.Employees.Add(newEmployee);
                        await _context.SaveChangesAsync();

                        // Assuming you have a method to get the logo as byte array from your business settings
                        byte[] businessLogoBytes = setting.BusinessLogo; // Already a byte array from your settings

                        using (var memoryStream = new MemoryStream(businessLogoBytes))
                        {
                            // Create an image resource from the memory stream
                            var imageResource = new LinkedResource(memoryStream, MediaTypeNames.Image.Jpeg)
                            {
                                ContentId = "business_logo", // This should match the cid in the email body
                                TransferEncoding = TransferEncoding.Base64
                            };

                            var message = new MailMessage();
                            message.From = new MailAddress(setting.ContactEmail); // Set your "from" email
                            message.To.Add(employee.Email);
                            message.Subject = "Welcome to the Team!";
                            var htmlBody = $@"
                                    <html>
                                    <body style='font-family: Arial, sans-serif; text-align: center;'>
                                        <div style='border: 1px solid #ddd; padding: 20px; border-radius: 5px;'>
                                            <h2 style='color: #4CAF50;'>Welcome to {businessName}!</h2>
                                            <p>Dear {employee.FirstName} {employee.LastName},</p>
                                            <p>We are excited to have you as part of our team! Your account has been created successfully.</p>
                                            <p><strong>Your Generated Password:</strong> <span style='color: #FF5733;'>{generatedPassword}</span></p>
                                            <p>Please change your password after logging in to ensure your account's security.</p>
                                            <p>If you have any questions or need assistance, feel free to reach out to us.</p>
                                            <hr>
                                        
                                        </div>
                                      <p style='margin-top: 20px;'>Best Regards,<br/>
                                        {businessName} Team</p>
                                        <p><a href='mailto:{setting.ContactEmail}'>{setting.ContactEmail}</a></p>
                                        <p style='color: #FF0000;'><{businessName}</p>
                                        <p style='color: #4CAF50;'><{business.Slogan}</p>
                                        <p>{business.Street},{business.City},{business.PostalCode}</p>
                                        <p>{business.StateProvince},{business.Country}</p>
                                        <p>{setting.ContactPhone}</p>
                                        <img src='cid:business_logo' alt='Business Logo' style='width: 100px; height: auto;' />
                                    </body>
                                    </html>";

                            // Send the email


                            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                            htmlView.LinkedResources.Add(imageResource);

                            message.AlternateViews.Add(htmlView);

                            await _emailSender.SendEmailAsync(employee.Email, "Welcome to the Team!", htmlBody);
                        }

                        var messages = $"Employee {employee.FirstName} {employee.LastName} was updated.";

                        // Create the notification after employee is deleted
                        await CreateEmployeeNotificationAsync("updated", employee, messages);
                        TempData["generatedPassword"] = generatedPassword;
                        TempData["created"] = "Employee created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "Error creating user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                        await SetLayoutData();
                        return View(employee);
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    TempData["Error"] = $"A database error occurred while creating the employee: {dbEx.InnerException?.Message ?? dbEx.Message}";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An unexpected error occurred while creating the employee: {ex.Message}";
                }

                await SetLayoutData();
                return View(employee);
            }

            TempData["Error"] = "Please ensure all required fields are filled out correctly.";
            await SetLayoutData();
            return View(employee);
        }




        // GET: Employee Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Employee ID is not provided.";
                await SetLayoutData();
                await EmployeeNotification();
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
            }

            // Additional properties to store ModifiedBy employee's information
            string modifiedByFirstName = "Unknown";
            string modifiedByLastName = "User";

            // Check if ModifiedBy is not null or empty
            if (!string.IsNullOrEmpty(employee.ModifiedBy))
            {
                // Retrieve the user details based on the ModifiedBy email
                var user = await _userManager.FindByEmailAsync(employee.ModifiedBy);

                if (user != null)
                {
                    // Fetch the employee details based on the email
                    var employeeDetails = await _context.Employees
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Email == employee.ModifiedBy && !e.IsDeleted);

                    if (employeeDetails != null)
                    {
                        // Assign the FirstName and LastName for the ModifiedBy user
                        modifiedByFirstName = employeeDetails.FirstName;
                        modifiedByLastName = employeeDetails.LastName;
                    }
                }
            }

            // Pass ModifiedBy info via ViewBag or a view model
            ViewBag.ModifiedByFirstName = modifiedByFirstName;
            ViewBag.ModifiedByLastName = modifiedByLastName;

            await SetLayoutData();
            await EmployeeNotification();
            return View(employee);
        }








        // GET: Employee/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Employee ID is not provided.";
                await SetLayoutData();
                await EmployeeNotification();
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
            }
            await SetLayoutData();
            await EmployeeNotification();
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile profilePhoto)
        {
            if (id != employee.EmployeeId)
            {
                TempData["Error"] = "Employee ID mismatch.";
                await SetLayoutData();
                await EmployeeNotification();
                return RedirectToAction(nameof(Index));
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
                    await EmployeeNotification();
                    return RedirectToAction(nameof(Index));
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
                existingEmployee.ModifiedDate = DateTime.Now;
                existingEmployee.ModifiedBy = User.Identity.Name; // Set modified by user

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
                await EmployeeNotification();
                _context.Update(existingEmployee);
                await _context.SaveChangesAsync();
                var message = $"Employee {employee.FirstName} {employee.LastName} was updated.";

                // Create the notification after employee is deleted
                await CreateEmployeeNotificationAsync("updated", employee, message);
                TempData["updated"] = "Employee updated successfully!";
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
            }
        }





        // GET: Employee Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Employee ID is not provided.";
                await SetLayoutData();
                await EmployeeNotification();
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employees
                .AsNoTracking().FirstOrDefaultAsync(m => m.EmployeeId == id && !m.IsDeleted);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                await SetLayoutData();
                await EmployeeNotification();
                return RedirectToAction(nameof(Index));
            }
            await SetLayoutData();
            await EmployeeNotification();
            return View(employee);
        }

        // POST: Employee Delete Confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || employee.IsDeleted)
            {
                TempData["Error"] = "Employee not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await SetLayoutData();
                await EmployeeNotification();
                employee.IsDeleted = true;
                employee.ModifiedDate = DateTime.Now;
                employee.ModifiedBy = User.Identity.Name;
                _context.Employees.Update(employee);
                await SetLayoutData();
                await EmployeeNotification();
                await _context.SaveChangesAsync();
                var message = $"Employee {employee.FirstName} {employee.LastName} was deleted.";

                // Create the notification after employee is deleted
                await CreateEmployeeNotificationAsync("delete", employee, message);
                TempData["deleted"] = "Employee deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"An error occurred while deleting the employee: {ex.Message}";
                return View(employee);
            }
        }

        // POST: Employee/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
            }

            if (!employee.IsDeleted)
            {
                TempData["Error"] = "Employee is not deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Restore the employee
                employee.IsDeleted = false;
                employee.ModifiedDate = DateTime.Now;
                employee.ModifiedBy = User.Identity.Name;
                await _context.SaveChangesAsync();
                var message = $"Employee {employee.FirstName} {employee.LastName} was restored.";

                // Create the notification after employee is deleted
                await CreateEmployeeNotificationAsync("restored", employee, message);
                TempData["Success"] = "Employee restored successfully!";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"An error occurred while restoring the employee: {ex.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }



        private string GenerateRandomPassword()
        {
            const int passwordLength = 12;
            const string upperChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*?_-";
            const string allValidChars = upperChars + lowerChars + numberChars + specialChars;

            string password;

            do
            {
                password = GeneratePassword(passwordLength, upperChars, lowerChars, numberChars, specialChars, allValidChars);
            }
            while (!IsValidPassword(password)); // Keep generating until it meets all criteria

            return password;
        }

        private string GeneratePassword(int length, string upperChars, string lowerChars, string numberChars, string specialChars, string allValidChars)
        {
            var password = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                // Ensure at least one character from each required group
                password.Append(upperChars[randomBytes[0] % upperChars.Length]);
                password.Append(lowerChars[randomBytes[1] % lowerChars.Length]);
                password.Append(numberChars[randomBytes[2] % numberChars.Length]);
                password.Append(specialChars[randomBytes[3] % specialChars.Length]);

                // Fill the rest of the password with random characters
                for (int i = 4; i < length; i++)
                {
                    int index = randomBytes[i] % allValidChars.Length;
                    password.Append(allValidChars[index]);
                }
            }

            return ShufflePassword(password.ToString());
        }

        private static bool IsValidPassword(string password)
        {
            // Check if the password contains at least one of each required character type
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => "!@#$%^&*?_-".Contains(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private static string ShufflePassword(string input)
        {
            var array = input.ToCharArray();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[array.Length];
                rng.GetBytes(randomBytes);

                for (int i = array.Length - 1; i > 0; i--)
                {
                    int j = randomBytes[i] % (i + 1);
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }

            return new string(array);
        }


        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id && !e.IsDeleted);
        }


    }
}
