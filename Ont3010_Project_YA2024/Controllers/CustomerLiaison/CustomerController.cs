using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ont3010_Project_YA2024.Controllers.Admin
{
    [Authorize(Roles = "Administrator, Customer Liaison")]
    public class CustomerController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        private readonly Data.Helpers.IEmailSender _emailSender;

        public CustomerController(BusinessService businessService, ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager,
            Data.Helpers.IEmailSender emailSender, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        private void SetLayoutBasedOnRole()
        {
            if (User.IsInRole("Administrator"))
            {
                ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            }
            else if (User.IsInRole("Customer Liaison"))
            {
                ViewData["Layout"] = "~/Views/Shared/_CustomerLiaisonLayout.cshtml";
            }
            else
            {
                ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
            }
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder, string sortDirection, int page = 1)
        {
            SetLayoutBasedOnRole();
            await SetLayoutData();
            await EmployeeNotification();
          

            // Set default values if null or empty
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "asc" : sortDirection;
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "FirstName" : sortOrder;

            // Set ViewData for sort and search states
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentDirection"] = sortDirection;
            ViewData["CurrentFilter"] = searchString;

            // Initialize the customer query
            var customers = _context.Customers
           .Include(c => c.FridgeAllocations)  // Include FridgeAllocations
           .Include(c => c.ProcessAllocations)  // Include ProcessAllocations
           .AsQueryable();

            // Apply search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchTerms = searchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                customers = customers.Where(c =>
                    c.FirstName.Contains(searchString) ||
                    c.LastName.Contains(searchString) ||
                    (searchTerms.Length > 1 &&
                     searchTerms.Any(term => c.FirstName.Contains(term)) &&
                     searchTerms.Any(term => c.LastName.Contains(term)))
                );
            }

            // Apply sorting
            customers = SortCustomers(customers, sortOrder, sortDirection);

            // Pagination logic
            int pageSize = 10;
            var totalCustomers = await customers.CountAsync();
            var pagedList = await customers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass total pages and current page to the view
            ViewBag.TotalPages = (int)Math.Ceiling(totalCustomers / (double)pageSize);
            ViewBag.CurrentPage = page;

            // Return the paged, sorted, and filtered list of customers
            return View(pagedList);
        }

        private IQueryable<Customer> SortCustomers(IQueryable<Customer> customers, string sortOrder, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            // Apply sorting based on the sortOrder and sortDirection parameters
            return sortOrder.ToLower() switch
            {
                "firstname" => isAscending ? customers.OrderBy(c => c.FirstName) : customers.OrderByDescending(c => c.FirstName),
                "lastname" => isAscending ? customers.OrderBy(c => c.LastName) : customers.OrderByDescending(c => c.LastName),
                "emailaddress" => isAscending ? customers.OrderBy(c => c.EmailAddress) : customers.OrderByDescending(c => c.EmailAddress),
                "companyname" => isAscending ? customers.OrderBy(c => c.BusinessName) : customers.OrderByDescending(c => c.BusinessName),
                _ => isAscending ? customers.OrderBy(c => c.FirstName) : customers.OrderByDescending(c => c.FirstName),
            };
        }


        private bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".tif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024; // 5 MB size limit
        }


        public async Task<IActionResult> Create()
        {
            SetLayoutBasedOnRole();
            await SetLayoutData(); // Ensure layout data is set before rendering the view
            await EmployeeNotification();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer, IFormFile profilePhoto)
        {
            if (ModelState.IsValid)
            {
                // Set layout and render view in case of validation errors
                SetLayoutBasedOnRole();
                await SetLayoutData();
                await EmployeeNotification();
            
                return View(customer);
            }

            try
            {
                // Handle profile photo upload
                if (profilePhoto != null && profilePhoto.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await profilePhoto.CopyToAsync(memoryStream);
                        customer.ProfilePhoto = memoryStream.ToArray();
                    }
                }

                // Check if email already exists
                if (EmailExists(customer.EmailAddress))
                {
                    TempData["Error"] = "A customer with this email already exists.";
                    SetLayoutBasedOnRole();
                    await SetLayoutData();
                    return View(customer);
                }

                // Generate a random password
                var generatedPassword = GenerateRandomPassword();
                var setting = await _businessService.GetSettingAsync();
                var businessName = setting?.BusinessName ?? "Default Company Name";

                // Create new customer object
                var newCustomer = new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Title = customer.Title,
                    EmailAddress = customer.EmailAddress,
                    PhoneNumber = customer.PhoneNumber,
                    BusinessRole = customer.BusinessRole ?? "Unknown Role", // Fallback for optional fields
                    BusinessName = customer.BusinessName,
                    BusinessEmailAddress = customer.BusinessEmailAddress,
                    BusinessPhoneNumber = customer.BusinessPhoneNumber,
                    BusinessType = customer.BusinessType ?? "Unknown",
                    Industry = customer.Industry ?? "Unspecified",
                    StreetAddress = customer.StreetAddress,
                    City = customer.City,
                    PostalCode = customer.PostalCode,
                    Province = customer.Province,
                    Country = customer.Country,
                    Password = generatedPassword,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "System",
                    IsDeleted = false
                };

                // Retrieve Employee ID based on email
                int? employeeId = null; // Initialize to null
                if (!string.IsNullOrEmpty(customer.CreatedBy))
                {
                    string normalizedEmail = customer.CreatedBy.Trim().ToLower();
                    var user = await _userManager.FindByEmailAsync(normalizedEmail);

                    if (user != null)
                    {
                        // Fetch the Employee ID
                        var employee = await _context.Employees
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.Email == normalizedEmail && !e.IsDeleted);

                        if (employee != null)
                        {
                            employeeId = employee.EmployeeId; // Get the Employee ID
                        }
                    }
                }

                // Add customer to the database
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                // Create the notification with Employee ID
                if (employeeId.HasValue)
                {
                    var notification = new Notification
                    {
                        Message = $"New customer added: {newCustomer.FirstName} {newCustomer.LastName}",
                        ActionBy = User.Identity?.Name ?? "System",
                        EmployeeId = employeeId.Value, // Set the retrieved Employee ID
                        CustomerId = newCustomer.CustomerId, // Assuming you have a way to set this
                        Date = DateTime.Now,
                        IsRead = false
                    };

                    // Add the notification to the context and save
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                // Assign user in Identity system
                var identityUser = new IdentityUser
                {
                    UserName = customer.EmailAddress,
                    Email = customer.EmailAddress
                };

                // Create user in Identity system with generated password
                var createUserResult = await _userManager.CreateAsync(identityUser, generatedPassword);
                if (createUserResult.Succeeded)
                {
                    // Confirm the user's email
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    await _userManager.ConfirmEmailAsync(identityUser, emailConfirmationToken);

                    // Assign "Customer" role
                    var addRoleResult = await _userManager.AddToRoleAsync(identityUser, "Customer");
                    if (addRoleResult.Succeeded)
                    {
                        // Sending the welcome email
                        byte[] businessLogoBytes = setting.BusinessLogo;

                        using (var memoryStream = new MemoryStream(businessLogoBytes))
                        {
                            var imageResource = new LinkedResource(memoryStream, MediaTypeNames.Image.Jpeg)
                            {
                                ContentId = "business_logo",
                                TransferEncoding = TransferEncoding.Base64
                            };

                            var message = new MailMessage();
                            message.From = new MailAddress(setting.ContactEmail, businessName);
                            message.To.Add(customer.EmailAddress);
                            message.Subject = $"Welcome to {businessName}";

                            var htmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; text-align: center;'>
                            <div style='border: 1px solid #ddd; padding: 20px; border-radius: 5px;'>
                                <h2 style='color: #4CAF50;'>Welcome to {businessName}!</h2>
                                <p>Dear {customer.FirstName} {customer.LastName},</p>
                                <p>Your account has been created successfully.</p>
                                <p><strong>Your Temporary Password:</strong> <span style='color: #FF5733;'>{generatedPassword}</span></p>
                                <p>Please change your password after logging in for security.</p>
                                <hr>
                                <p>Best Regards,<br/>{businessName} Team</p>
                            </div>
                            <img src='cid:business_logo' alt='Business Logo' style='width: 100px; height: auto;' />
                        </body>
                        </html>";

                            var plainTextBody = $@"
                        Welcome to {businessName}!

                        Dear {customer.FirstName} {customer.LastName},

                        Your account has been created successfully.
                        Your Temporary Password: {generatedPassword}

                        Please change your password after logging in for security.

                        Best Regards,
                        {businessName} Team";

                            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                            htmlView.LinkedResources.Add(imageResource);
                            message.AlternateViews.Add(htmlView);

                            var plainTextView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, MediaTypeNames.Text.Plain);
                            message.AlternateViews.Add(plainTextView);

                            message.Headers.Add("X-Priority", "1 (Highest)");
                            message.Headers.Add("X-Mailer", "MyBusinessMailer");
                            message.Headers.Add("X-Sender", setting.ContactEmail);

                            await _emailSender.SendEmailAsync(customer.EmailAddress, message.Subject, htmlBody);
                        }

                        TempData["created"] = "Customer created and role assigned successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "Customer created, but there was an issue assigning the role.";
                    }
                }
                else
                {
                    TempData["Error"] = "There was an issue creating the user in the system.";
                }
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = $"A database error occurred while creating the customer: {dbEx.Message}";
                if (dbEx.InnerException != null)
                {
                    TempData["Error"] += $" Inner exception: {dbEx.InnerException.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred while creating the customer: {ex.Message}";
            }

            // Return view with errors if there are issues during the process
            SetLayoutBasedOnRole();
            await EmployeeNotification();
       
            await SetLayoutData();
            return View(customer);
        }





        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound("Customer ID is not provided.");
            }

            var customer = await _context.Customers
                .Include(c => c.FridgeAllocations)  // Include FridgeAllocations
                .Include(c => c.ProcessAllocations) // Include ProcessAllocations
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Normalize the CreatedBy email for the Customer entity
            if (!string.IsNullOrEmpty(customer.CreatedBy))
            {
                string normalizedEmail = customer.CreatedBy.Trim().ToLower();
                var user = await _userManager.FindByEmailAsync(normalizedEmail);

                if (user != null)
                {
                    var employee = await _context.Employees
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Email == normalizedEmail && !e.IsDeleted);
                    if (employee != null)
                    {
                        customer.CreatedBy = $"{employee.FirstName} {employee.LastName}";
                    }
                }
            }

            // Fetch user details for FridgeAllocations
            foreach (var fridgeAllocation in customer.FridgeAllocations)
            {
                if (!string.IsNullOrEmpty(fridgeAllocation.CreatedBy))
                {
                    // Normalize the CreatedBy email
                    string normalizedEmail = fridgeAllocation.CreatedBy.Trim().ToLower();

                    var employee = await _context.Employees
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Email.Trim().ToLower() == normalizedEmail && !e.IsDeleted);
                    if (employee != null)
                    {
                        fridgeAllocation.CreatedBy = $"{employee.FirstName} {employee.LastName}";
                    }
                }
            }


            foreach (var processAllocation in customer.ProcessAllocations)
            {
                if (!string.IsNullOrEmpty(processAllocation.LastModifiedBy))
                {
                    // Normalize the LastModifiedBy email
                    string normalizedEmail = processAllocation.LastModifiedBy.Trim().ToLower();

                    // Fetch the employee based on the normalized email
                    var employee = await _context.Employees
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Email.Trim().ToLower() == normalizedEmail && !e.IsDeleted);

                    // If the employee is found, replace the email with their full name
                    if (employee != null)
                    {
                        processAllocation.LastModifiedBy = $"{employee.FirstName} {employee.LastName}";
                    }
                    else
                    {
                        // Optionally, handle cases where the employee is not found
                        processAllocation.LastModifiedBy = "Unknown User";
                       
                    }
                }
            }




            SetLayoutBasedOnRole();
            await SetLayoutData(); // Ensure layout data is set before rendering the view
            await EmployeeNotification();
         
            return View(customer);
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

            SetLayoutBasedOnRole();
            await EmployeeNotification();
           
            await SetLayoutData(); // Ensure layout data is set before rendering the view
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer, IFormFile profilePhoto)
        {
            if (id != customer.CustomerId)
            {
                return NotFound("Customer ID mismatch.");
            }

            // Retrieve the existing customer from the database
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found.");
            }

            // Check model state before proceeding
            if (!ModelState.IsValid)
            {
                // Check for existing email
                if (EmailExists(customer.EmailAddress, id))
                {
                    TempData["Error"] = "A customer with this email already exists.";
                    SetLayoutBasedOnRole();
                    await EmployeeNotification();
                    await SetLayoutData();
                    return View(customer); // Return the view with validation errors
                }

                // Update only necessary fields, ignore password and created fields
                try
                {
                    // Update fields excluding password, CreatedBy, and CreatedDate
                    // Update all properties
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
                            return View(customer);  // Return to the same view with the model to display validation errors
                        }
                    }
                    // Save changes to the database
                    _context.Customers.Update(existingCustomer);
                    await _context.SaveChangesAsync();
                    TempData["updated"] = "Customer updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound("Customer not found.");
                    }
                    else
                    {
                        throw; // Handle concurrency issue
                    }
                }
            }

            // If we got this far, something failed; redisplay form
            SetLayoutBasedOnRole();
            await EmployeeNotification();
           
            await SetLayoutData(); // Ensure layout data is set before rendering the view with validation errors
            return View(customer);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData(); // Ensure layout data is set before rendering the view
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                customer.IsDeleted = true;
                _context.Customers.Update(customer);
                TempData["deleted"] = "Customer deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Something went wrong while deleting the customer.";
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
         
            await SetLayoutData(); // Ensure layout data is set before rendering the view with errors
            return View(customer);
        }

        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null || !customer.IsDeleted)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Set IsDeleted to false to restore the customer
                customer.IsDeleted = false;
                _context.Customers.Update(customer); // Update the customer
                await _context.SaveChangesAsync();
                TempData["restored"] = "Customer restored successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Something went wrong while restoring the customer.";
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData(); // Ensure layout data is set before rendering the view with errors
            return View(customer);
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
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
