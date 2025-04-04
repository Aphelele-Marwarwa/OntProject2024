using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.InventoryLiaison;

namespace Ont3010_Project_YA2024.Controllers.InventoryLiaison
{
    [Authorize(Roles = "Administrator, Inventory Liaison")]
    public class ScrappedFridgeController : BaseController
    {

        private readonly ApplicationDbContext _context;

        public ScrappedFridgeController(BusinessService businessService, ApplicationDbContext context,INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _context = context;
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

        // GET: ScrappedFridge
        [HttpGet]
        public async Task<IActionResult> Index(string searchString,int? fridgeId, DateTime? scrapeDate, string sortOrder, string sortDirection, int page = 1)
        {
            SetLayoutBasedOnRole();
            await SetLayoutData();
            await EmployeeNotification();

            // Set default sorting values
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "asc" : sortDirection;
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "ScrapeDate" : sortOrder;

            // Set ViewData for current sort and filter
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentDirection"] = sortDirection;
            ViewData["CurrentFridgeId"] = fridgeId;
            ViewData["CurrentScrapeDate"] = scrapeDate?.ToString("yyyy-MM-dd"); // Format the date for display
           
            // Retrieve all scrapped fridges with their Fridge details included
            var scrappedFridgesQuery = _context.ScrappedFridges.Include(s => s.Fridge).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                scrappedFridgesQuery = scrappedFridgesQuery.Where(f =>
                    f.Fridge.SerialNumber.Contains(searchString) || // Access SerialNumber from Fridge entity
                    f.FridgeId.ToString() == searchString);
            }
            // Search functionality
            if (fridgeId.HasValue)
            {
                scrappedFridgesQuery = scrappedFridgesQuery.Where(s => s.Fridge.FridgeId == fridgeId.Value);
            }

            if (scrapeDate.HasValue)
            {
                scrappedFridgesQuery = scrappedFridgesQuery.Where(s => s.CreatedDate.Date == scrapeDate.Value.Date);
            }

            // Apply sorting
            scrappedFridgesQuery = SortScrappedFridges(scrappedFridgesQuery, sortOrder, sortDirection);

            // Pagination
            int pageSize = 10;
            var totalFridges = await scrappedFridgesQuery.CountAsync();
            var pagedList = await scrappedFridgesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Set pagination data in ViewBag
            ViewBag.TotalPages = (int)Math.Ceiling(totalFridges / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedList);
        }

        // Sorting function
        private IQueryable<ScrappedFridge> SortScrappedFridges(IQueryable<ScrappedFridge> scrappedFridges, string sortOrder, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            return sortOrder.ToLower() switch
            {
                "fridgeid" => isAscending ? scrappedFridges.OrderBy(s => s.Fridge.FridgeId) : scrappedFridges.OrderByDescending(s => s.Fridge.FridgeId),
                "scrapedate" => isAscending ? scrappedFridges.OrderBy(s => s.CreatedDate) : scrappedFridges.OrderByDescending(s => s.CreatedDate),
                _ => isAscending ? scrappedFridges.OrderBy(s => s.CreatedDate) : scrappedFridges.OrderByDescending(s => s.CreatedDate)
            };
        }



        // GET: ScrappedFridge/Create
        [HttpGet]
        public async Task<IActionResult> Create(int id, string serialNumber)
        {
            if (id <= 0 || string.IsNullOrEmpty(serialNumber))
            {
                return NotFound(); // Handle invalid parameters
            }

            // Attempt to find the fridge using both parts of the composite key
            var fridge = await _context.Fridges
                .FirstOrDefaultAsync(f => f.FridgeId == id && f.SerialNumber == serialNumber);

            if (fridge == null)
            {
                return NotFound(); // Handle not found fridge
            }

            // Set any necessary layout data
            SetLayoutBasedOnRole();
            await SetLayoutData();
            await EmployeeNotification();

            // Prepare the scrapped fridge entry
            var scrappedFridge = new ScrappedFridge
            {
                FridgeId = fridge.FridgeId,
                FridgeSerialNumber = fridge.SerialNumber // Ensure the serial number is taken from the fridge
            };

            return View(scrappedFridge); // Pass the new scrapped fridge entry to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScrappedFridge scrappedFridge, int id, string serialNumber)
        {
            // Try to find the fridge using both parts of the composite key (FridgeId and SerialNumber)
            var fridge = await _context.Fridges
                .FirstOrDefaultAsync(f => f.FridgeId == id && f.SerialNumber == serialNumber);

            if (fridge == null)
            {
                // Fridge not found, add a ModelState error and return to view
                ModelState.AddModelError("", "Fridge not found.");
            }
            else
            {
                // Check if the ModelState is valid
                if (!ModelState.IsValid)
                {
                    try
                    {
                        // Update the Fridge to indicate it's scrapped
                        fridge.IsScrapped = true;
                        fridge.IsInStock = false;
                        fridge.IsAllocated = false;
                        fridge.IsDeleted = false; // Keep the record in the system but mark as scrapped
                        _context.Update(fridge);

                        // Set properties for the scrapped fridge entry
                        scrappedFridge.CreatedBy = User.Identity.Name;
                        scrappedFridge.CreatedDate = DateTime.Now;
                        scrappedFridge.ScrapDate = DateTime.Now;
                        scrappedFridge.FridgeSerialNumber = fridge.SerialNumber; // Correctly set FridgeSerialNumber

                        // Get EmployeeId based on the logged-in user's email
                        scrappedFridge.EmployeeId = _context.Employees
                            .Where(e => e.Email == User.Identity.Name)
                            .Select(e => e.EmployeeId)
                            .FirstOrDefault();

                        // Fetch the employee details
                        var actionByEmployee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Email == User.Identity.Name);

                        // Validate EmployeeId
                        if (scrappedFridge.EmployeeId == 0 || actionByEmployee == null)
                        {
                            // If EmployeeId is invalid, show an error
                            ModelState.AddModelError("", "Employee ID is not valid.");
                        }
                        else
                        {
                            // If all validations pass, save the scrapped fridge entry
                            _context.ScrappedFridges.Add(scrappedFridge);
                            await _context.SaveChangesAsync();

                            // Set a success message in TempData and redirect to Index
                            TempData["success"] = "Fridge scrapped successfully!";

                            // Create the notification after employee is scrapped
                            var message = $"Fridge {fridge.SerialNumber} was scrapped by {actionByEmployee.FirstName} {actionByEmployee.LastName}.";
                            await CreateEmployeeNotificationAsync("scrapped", actionByEmployee, message);

                            return RedirectToAction(nameof(Index));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log and display error message
                        ModelState.AddModelError("", "An error occurred while saving changes: " + ex.Message);
                    }
                }
            }

            // If we reach here, something went wrong, so we need to repopulate the view data
            SetLayoutBasedOnRole();  // Populate layout information based on the user's role
            await SetLayoutData();    // Any additional layout data (e.g., ViewBag, ViewData settings)
            await EmployeeNotification(); // Re-populate employee-specific notifications

            // Return to the view with the current scrappedFridge model
            return View(scrappedFridge);
        }






        [HttpPost]
        public async Task<IActionResult> Restore(int FridgeId, string FridgeSerialNumber, int EmployeeId)
        {
            try
            {
                // Find the fridge by both FridgeId and SerialNumber (composite key)
                var fridge = await _context.Fridges.FindAsync(FridgeId, FridgeSerialNumber); // Pass FridgeId (int) first, then SerialNumber (string)

                if (fridge == null)
                {
                    TempData["error"] = $"Fridge with ID {FridgeId} and Serial Number {FridgeSerialNumber} not found";
                    return RedirectToAction(nameof(Index));
                }

                // Fetch the employee details based on the logged-in user's email
                var actionByEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == User.Identity.Name);

                // Additional validation could go here to verify EmployeeId, if necessary

                // Update only the necessary field
                fridge.IsScrapped = false;
                fridge.IsInStock = true;
                fridge.IsAllocated = false;
                fridge.IsDeleted = false;

                var scrappedFridge = await _context.ScrappedFridges
                    .FirstOrDefaultAsync(s => s.FridgeId == FridgeId && s.FridgeSerialNumber == FridgeSerialNumber);

                if (scrappedFridge != null)
                {
                    // Remove the scrapped fridge record
                    _context.ScrappedFridges.Remove(scrappedFridge);
                }

                await _context.SaveChangesAsync();

                // Create the notification after restoring the fridge
                if (actionByEmployee != null)
                {
                    var message = $"Fridge {fridge.SerialNumber} was restored by {actionByEmployee.FirstName} {actionByEmployee.LastName}.";
                    await CreateEmployeeNotificationAsync("restored", actionByEmployee, message);
                }

                TempData["success"] = "Fridge restored successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
