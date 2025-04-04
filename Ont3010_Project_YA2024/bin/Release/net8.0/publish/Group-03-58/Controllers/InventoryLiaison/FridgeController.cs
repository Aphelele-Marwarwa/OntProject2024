using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using Ont3010_Project_YA2024.Data.Notifications;
using System.Text;
using Ont3010_Project_YA2024.Models;

namespace Ont3010_Project_YA2024.Controllers.InventoryLiaison
{
    [Authorize(Roles = "Administrator, Inventory Liaison")]
    public class FridgeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly INotificationService _notificationService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;

        public FridgeController(BusinessService businessService, ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager
            , IWebHostEnvironment hostEnvironment, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _notificationService = notificationService;

        }

       

        private void SetLayoutBasedOnRole()
        {
            if (User.IsInRole("Administrator"))
            {
                ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            }
            else if (User.IsInRole("Inventory Liaison"))
            {
                ViewData["Layout"] = "~/Views/Shared/_InventoryLiaisonLayout.cshtml";
            }
            else
            {
                ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
            }
        }

        private bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".tif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024; // 5 MB size limit
        }


        private bool IsValidDocument(IFormFile file)
        {
            var allowedExtensions = new[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".ppt", ".pptx" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 10 * 1024 * 1024; // 10 MB size limit
        }

        // GET: Fridge
        public async Task<IActionResult> Index(string searchString, string sortOrder, string sortDirection, int? fridgeId, string serialNumber, int page = 1)
        {
            await SetLayoutData();
            await EmployeeNotification();
            SetLayoutBasedOnRole();

           

            // Set default sorting values
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "asc" : sortDirection;
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "SerialNumber" : sortOrder;

            // Set ViewData for current sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentDirection"] = sortDirection;
            ViewData["CurrentFilter"] = searchString;

            // Retrieve all fridges
            var fridgesQuery = _context.Fridges
                .Where(f => f.IsInStock == true && f.IsScrapped == false && f.IsDeleted == false)
                .AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                fridgesQuery = fridgesQuery.Where(f =>
                    f.SerialNumber.Contains(searchString) || // Access SerialNumber from Fridge entity
                    f.FridgeId.ToString() == searchString);
            }

            // Apply sorting
            fridgesQuery = SortFridges(fridgesQuery, sortOrder, sortDirection);

            // Pagination
            int pageSize = 10;
            var totalFridges = await fridgesQuery.CountAsync();
            var pagedList = await fridgesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalFridges / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedList);
        }

        private IQueryable<Fridge> SortFridges(IQueryable<Fridge> fridges, string sortOrder, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            return sortOrder.ToLower() switch
            {
                "serialnumber" => isAscending ? fridges.OrderBy(f => f.SerialNumber) : fridges.OrderByDescending(f => f.SerialNumber),
                "fridgeid" => isAscending ? fridges.OrderBy(f => f.FridgeId) : fridges.OrderByDescending(f => f.FridgeId),
                // Add other properties to sort by as needed
                _ => isAscending ? fridges.OrderBy(f => f.SerialNumber) : fridges.OrderByDescending(f => f.SerialNumber)
            };
        }






        // GET: Fridge/Create
        public async Task<IActionResult> Create()
        {
          

            SetLayoutBasedOnRole();
            await SetLayoutData();
            await EmployeeNotification();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fridge fridge, IFormFile file, IFormFile fridgeImage)
        {
            // Check if the document file is provided
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Document is required.");
                SetLayoutBasedOnRole();
                await EmployeeNotification();
                await SetLayoutData();
                return View(fridge);
            }

            // Validate the document file
            var allowedExtensions = new[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".ppt", ".pptx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("file", "Only Word, Excel, PDF, and PowerPoint files are allowed.");
                SetLayoutBasedOnRole();
                await SetLayoutData();
                await EmployeeNotification();
                return View(fridge);
            }

            // Check if fridge with same serial number exists
            var exists = await _context.Fridges.AnyAsync(f => f.SerialNumber == fridge.SerialNumber);
            if (exists)
            {
                ModelState.AddModelError("SerialNumber", "A fridge with this serial number already exists.");
                SetLayoutBasedOnRole();
                await SetLayoutData();
                await EmployeeNotification();
                return View(fridge);
            }

            // Date validation
            if (fridge.WarrantyStartDate < DateTime.Now)
            {
                ModelState.AddModelError("WarrantyStartDate", "Warranty start date cannot be in the past.");
            }
            if (fridge.WarrantyEndDate < DateTime.Now)
            {
                ModelState.AddModelError("WarrantyEndDate", "Warranty end date cannot be in the past.");
            }
            if (fridge.WarrantyEndDate < fridge.WarrantyStartDate)
            {
                ModelState.AddModelError("WarrantyEndDate", "Warranty end date cannot be before the start date.");
            }

            // Process the document file
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fridge.DeliveryDocumentation = memoryStream.ToArray();
                fridge.DeliveryDocumentationFileName = file.FileName;
            }

            // Process fridge image if provided
            if (fridgeImage != null && fridgeImage.Length > 0)
            {
                if (IsValidImage(fridgeImage))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fridgeImage.CopyToAsync(memoryStream);
                        fridge.FridgeImage = memoryStream.ToArray();
                        fridge.FridgeImageFileName = fridgeImage.FileName;
                    }
                }
                else
                {
                    ModelState.AddModelError("fridgeImage", "Only valid image files are allowed.");
                    SetLayoutBasedOnRole();
                    await SetLayoutData();
                    await EmployeeNotification();
                    return View(fridge);
                }
            }

            fridge.CreatedBy = User.Identity.Name;
            fridge.CreatedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                _context.Add(fridge);
                await _context.SaveChangesAsync();

                // Create Notification
                var actionByEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == User.Identity.Name);

                var actionBy = actionByEmployee != null
                    ? $"{actionByEmployee.FirstName} {actionByEmployee.LastName}"
                    : User.Identity.Name; // Fallback to email if name is not found

                var notification = new Notification
                {
                    Message = $"Fridge with serial number {fridge.SerialNumber} was created.",
                    ActionBy = actionBy,
                    Date = DateTime.Now
                };

                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                // Fetch all administrators
                var administrators = await _context.Employees
                    .Where(e => e.Role == "Administrator")
                    .ToListAsync();

                // Create EmployeeNotificationStatus for all administrators
                foreach (var admin in administrators)
                {
                    var status = new EmployeeNotificationStatus
                    {
                        EmployeeId = admin.EmployeeId,
                        NotificationId = notification.Id,
                        IsRead = false // Set IsRead to false by default for all admins
                    };

                    // Mark as read if the admin is the one who created the fridge
                    if (admin.Email == User.Identity.Name)
                    {
                        status.IsRead = true;
                    }

                    _context.EmployeeNotificationStatuses.Add(status);
                }

                // Save changes for notification statuses
                await _context.SaveChangesAsync();

                // Redirect to Index with success message
                TempData["created"] = "Fridge created successfully!";
                return RedirectToAction(nameof(Index));
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData();
            return View(fridge);
        }


        // POST: Fridge/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string serialNumber, Fridge fridge, IFormFile fridgeImage, IFormFile newDocument, string existingFridgeImage, string existingDocument)
        {
            if (id != fridge.FridgeId || serialNumber != fridge.SerialNumber)
            {
                return NotFound();
            }

            // Validate SerialNumber
            if (string.IsNullOrWhiteSpace(fridge.SerialNumber))
            {
                ModelState.AddModelError("SerialNumber", "Serial number is required.");
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    // Find existing fridge by its composite key (FridgeId and SerialNumber)
                    var existingFridge = await _context.Fridges
                        .AsNoTracking()  // Disable tracking since we will modify the serial number
                        .FirstOrDefaultAsync(f => f.FridgeId == id && f.SerialNumber == serialNumber);

                    if (existingFridge == null)
                    {
                        return NotFound();
                    }

                    // Detach the existing entity from the context
                    _context.Entry(existingFridge).State = EntityState.Detached;

                    // Update fridge properties
                    existingFridge.ModelType = fridge.ModelType;
                    existingFridge.Condition = fridge.Condition;
                    existingFridge.SupplierName = fridge.SupplierName;
                    existingFridge.SupplierContact = fridge.SupplierContact;
                    existingFridge.Note = fridge.Note;
                    existingFridge.IsInStock = fridge.IsInStock;
                    existingFridge.IsScrapped = fridge.IsScrapped;
                    existingFridge.IsAllocated = fridge.IsAllocated;
                    existingFridge.LastModifiedBy = User.Identity.Name;
                    existingFridge.LastModifiedDate = DateTime.Now;

                    // Update the serial number (since it's part of the composite key)
                    existingFridge.SerialNumber = fridge.SerialNumber;

                    // Process fridge image if provided
                    if (fridgeImage != null && fridgeImage.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await fridgeImage.CopyToAsync(memoryStream);
                            existingFridge.FridgeImage = memoryStream.ToArray();
                        }
                    }
                    else if (!string.IsNullOrEmpty(existingFridgeImage))
                    {
                        existingFridge.FridgeImage = Convert.FromBase64String(existingFridgeImage);
                    }

                    // Process document file if provided
                    if (newDocument != null && newDocument.Length > 0)
                    {
                        if (IsValidDocument(newDocument))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await newDocument.CopyToAsync(memoryStream);
                                existingFridge.DeliveryDocumentation = memoryStream.ToArray();
                                existingFridge.DeliveryDocumentationFileName = newDocument.FileName;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("newDocument", "Only valid document files are allowed.");
                            return View(existingFridge);
                        }
                    }
                    else if (!string.IsNullOrEmpty(existingDocument))
                    {
                        existingFridge.DeliveryDocumentation = Convert.FromBase64String(existingDocument);
                    }

                    // Attach the updated entity and mark it as modified
                    _context.Attach(existingFridge).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    TempData["updated"] = "Fridge updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FridgeExists(id, serialNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData();
            return View(fridge);
        }



        // GET: Fridge/Details/5
        public async Task<IActionResult> Details(int? id, string serialNumber)
        {
            // Check if ID or serial number is null
            if (id == null || string.IsNullOrEmpty(serialNumber))
            {
                TempData["Error"] = "Fridge ID or Serial Number is not provided.";
                await SetLayoutData();
                await EmployeeNotification();
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the fridge along with the employee and allocation details
            var fridge = await _context.Fridges
                .AsNoTracking()
                .Include(f => f.Employee) // Include the Employee navigation property
                .Include(f => f.FridgeAllocations) // Include the FridgeAllocations navigation property
                .ThenInclude(a => a.Customer) // Include the Customer navigation property from FridgeAllocations
                .FirstOrDefaultAsync(f => f.FridgeId == id && f.SerialNumber == serialNumber);

            // Check if the fridge was found
            if (fridge == null)
            {
                TempData["Error"] = "Fridge not found.";
                return RedirectToAction(nameof(Index));
            }

            // Initialize createdBy, allocatedBy, customerName, and allocationDate variables
            string createdBy = "Unknown";
            string allocatedBy = "Unknown";
            string customerName = "Unknown";
            DateTime? allocationDate = null; // Use nullable DateTime

            // Fetch the allocation that is related to this fridge if any
            var allocation = fridge.FridgeAllocations
                .OrderByDescending(a => a.CreatedDate) // Assuming you want the latest allocation
                .FirstOrDefault();

            if (allocation != null)
            {
                // Assign CreatedBy from the allocation to display
                createdBy = allocation.CreatedBy;

                // Fetch employee details who allocated the fridge
                var allocatingEmployee = await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployeeId == allocation.EmployeeId && !e.IsDeleted);

                if (allocatingEmployee != null)
                {
                    allocatedBy = $"{allocatingEmployee.FirstName} {allocatingEmployee.LastName}";
                }

                // Fetch the customer details associated with this allocation
                var customer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CustomerId == allocation.CustomerId);

                if (customer != null)
                {
                    customerName = $"{customer.FirstName} {customer.LastName}";
                }

                // Capture the AllocationDate from the allocation
                allocationDate = allocation.AllocationDate; // Assuming you have this property in your allocation model
            }

            // If LastModifiedBy is not null or empty, fetch the employee who modified the fridge
            if (!string.IsNullOrEmpty(fridge.LastModifiedBy))
            {
                // Retrieve the user details based on the LastModifiedBy email
                var user = await _userManager.FindByEmailAsync(fridge.LastModifiedBy);
                if (user != null)
                {
                    // Fetch employee details based on email
                    var employeeDetails = await _context.Employees
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Email == fridge.LastModifiedBy && !e.IsDeleted);

                    if (employeeDetails != null)
                    {
                        // Update the fridge's employee property with the found employee
                        fridge.Employee = employeeDetails;
                    }
                    else
                    {
                        // If employee details are not found, log the event
                        Console.WriteLine($"Employee details not found for email: {fridge.LastModifiedBy}");
                        fridge.Employee = new Employee { FirstName = "Unknown", LastName = "User" }; // Assign default values
                    }
                }
                else
                {
                    // Log if user not found
                    Console.WriteLine($"User not found for email: {fridge.LastModifiedBy}");
                    fridge.Employee = new Employee { FirstName = "Unknown", LastName = "User" }; // Assign default values
                }
            }
            else
            {
                fridge.Employee = new Employee { FirstName = "Unknown", LastName = "User" }; // Assign default values if LastModifiedBy is empty
            }

            // Set the ViewBag or Model for CreatedBy, AllocatedBy, CustomerName, and AllocationDate to be displayed in the view
            ViewBag.CreatedBy = createdBy;
            ViewBag.AllocatedBy = allocatedBy;
            ViewBag.CustomerName = customerName;
            ViewBag.AllocationDate = allocationDate; // Pass the AllocationDate to the view

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData();
            return View(fridge);
        }





        // GET: Fridge/Delete/5
        public async Task<IActionResult> Delete(int id, string serialNumber)
        {
            if (id <= 0 || string.IsNullOrEmpty(serialNumber))
            {
                return NotFound();
            }

            var fridge = await _context.Fridges
                .FirstOrDefaultAsync(m => m.FridgeId == id && m.SerialNumber == serialNumber);

            if (fridge == null)
            {
                return NotFound();
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData();
            return View(fridge);
        }

        // POST: Fridge/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string serialNumber)
        {
            var fridge = await _context.Fridges
                .FirstOrDefaultAsync(m => m.FridgeId == id && m.SerialNumber == serialNumber);

            if (fridge != null)
            {



                fridge.IsScrapped = false;
                fridge.IsInStock = false;
                fridge.IsAllocated = false;
                fridge.IsDeleted = true;
                fridge.LastModifiedBy = User.Identity.Name;
                fridge.LastModifiedDate = DateTime.Now;
                _context.Update(fridge);
                await _context.SaveChangesAsync();
                TempData["deleted"] = "Fridge deleted successfully!";

            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id, string serialNumber)
        {
            var fridge = await _context.Fridges
                .FirstOrDefaultAsync(m => m.FridgeId == id && m.SerialNumber == serialNumber);

            if (fridge != null)
            {
                fridge.IsScrapped = false;
                fridge.IsInStock = true;
                fridge.IsDeleted = false;
                fridge.IsDeleted = false;
                fridge.LastModifiedBy = User.Identity.Name;
                fridge.LastModifiedDate = DateTime.Now;
                _context.Update(fridge);
                await _context.SaveChangesAsync();
                TempData["deleted"] = "Fridge restored successfully!";
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
            await SetLayoutData();
            return RedirectToAction(nameof(Index));
        }


        private bool FridgeExists(int id, string serialNumber)
        {
            return _context.Fridges.Any(e => e.FridgeId == id && e.SerialNumber == serialNumber && !e.IsScrapped && e.IsInStock);
        }

        public IActionResult PreviewFile(int id, string serialNumber)
        {
            var fridge = _context.Fridges
                .FirstOrDefault(f => f.FridgeId == id && f.SerialNumber == serialNumber);

            if (fridge != null && fridge.DeliveryDocumentation != null)
            {
                var fileExtension = Path.GetExtension(fridge.DeliveryDocumentationFileName).ToLowerInvariant();
                string contentType;

                switch (fileExtension)
                {
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    default:
                        contentType = "application/octet-stream";
                        break;
                }

                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fridge.DeliveryDocumentationFileName}\"");
                return File(fridge.DeliveryDocumentation, contentType);
            }

            return NotFound();
        }


        //// Example implementation of a method to find a notification
        //public async Task<IActionResult> MarkNotificationAsRead(int id)
        //{
        //    await _notificationService.MarkAsReadAsync(id);
        //    return RedirectToAction("Index", "Notifications"); // Redirect to the notification listing or relevant page
        //}




    }
}