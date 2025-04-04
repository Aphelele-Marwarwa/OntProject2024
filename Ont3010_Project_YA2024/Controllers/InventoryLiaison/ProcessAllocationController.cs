using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.InventoryLiaison;

namespace Ont3010_Project_YA2024.Controllers.InventoryLiaison
{
    [Authorize(Roles = "Inventory Liaison")]
    public class ProcessAllocationController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ProcessAllocationController(BusinessService businessService,ApplicationDbContext context, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _context = context;
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
        // GET: ProcessAllocation/Index
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string sortOrder, string sortDirection, int page = 1)
        {
            await SetLayoutData();
            await EmployeeNotification();
            await CustomerNotification();

            // Set default sorting values
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "asc" : sortDirection;
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "DeliveryPickupDate" : sortOrder;

            // Set ViewData for current sort and filter
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentDirection"] = sortDirection;
            ViewData["SearchString"] = searchString;

            var allocations = _context.FridgeAllocations
                .Include(fa => fa.Customer)
                .Include(fa => fa.Fridge)
                .Where(fa => !fa.IsProcessed); // Show only unprocessed allocations

            // Add search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                allocations = allocations.Where(fa =>
                    fa.Customer.LastName.Contains(searchString) ||
                    fa.Fridge.SerialNumber.Contains(searchString)); // Search by customer name or fridge serial
            }

            var processAllocations = allocations.Select(fa => new ProcessAllocation
            {
                ProcessAllocationId = fa.FridgeAllocationId,
                FridgeAllocationId = fa.FridgeAllocationId,
                CustomerName = fa.Customer.FirstName,
                CustomerLast = fa.Customer.LastName,
                CustomerId = fa.Customer.CustomerId,
                SerialNumber = fa.Fridge.SerialNumber,
                FridgeId = fa.FridgeId,
                AllocationDate = fa.AllocationDate,
                SpecialInstructions = fa.SpecialInstructions,
                ApprovalStatus = fa.IsProcessed ? "Processed" : "Not Processed",
                ApprovalNote = "",
                FridgeAllocation = fa
            });

            // Apply sorting
            processAllocations = SortProcessAllocations(processAllocations, sortOrder, sortDirection);

            // Pagination
            int pageSize = 10;
            var totalAllocations = await processAllocations.CountAsync();
            var pagedList = await processAllocations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Set pagination data in ViewBag
            ViewBag.TotalPages = (int)Math.Ceiling(totalAllocations / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedList);
        }

        // Method to sort process allocations
        private IQueryable<ProcessAllocation> SortProcessAllocations(IQueryable<ProcessAllocation> allocations, string sortOrder, string sortDirection)
        {
            switch (sortOrder)
            {
                case "CustomerName":
                    allocations = sortDirection == "asc" ? allocations.OrderBy(pa => pa.CustomerName) : allocations.OrderByDescending(pa => pa.CustomerName);
                    break;
                case "SerialNumber":
                    allocations = sortDirection == "asc" ? allocations.OrderBy(pa => pa.SerialNumber) : allocations.OrderByDescending(pa => pa.SerialNumber);
                    break;
                case "FridgeId":
                    allocations = sortDirection == "asc" ? allocations.OrderBy(pa => pa.FridgeId) : allocations.OrderByDescending(pa => pa.FridgeId);
                    break;
                case "DeliveryPickupDate":
                    allocations = sortDirection == "asc" ? allocations.OrderBy(pa => pa.AllocationDate) : allocations.OrderByDescending(pa => pa.AllocationDate);
                    break;
                default:
                    allocations = allocations.OrderByDescending(pa => pa.AllocationDate); // Default sort order
                    break;
            }
            return allocations;
        }




        // GET: ProcessAllocation/Process/5
        public async Task<IActionResult> Process(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fridgeAllocation = await _context.FridgeAllocations
                .Include(fa => fa.Customer)
                .Include(fa => fa.Fridge)
                .FirstOrDefaultAsync(m => m.FridgeAllocationId == id);

            if (fridgeAllocation == null)
            {
                return NotFound();
            }

            var model = new ProcessAllocation
            {
                ProcessAllocationId = fridgeAllocation.FridgeAllocationId,
                FridgeAllocationId = fridgeAllocation.FridgeAllocationId,
                CustomerId = fridgeAllocation.Customer.CustomerId,
                CustomerName = fridgeAllocation.Customer.FirstName,
                CustomerLast = fridgeAllocation.Customer.LastName,
                FridgeId = fridgeAllocation.FridgeId,
                SerialNumber = fridgeAllocation.Fridge.SerialNumber,
                AllocationDate = fridgeAllocation.AllocationDate,
                SpecialInstructions = fridgeAllocation.SpecialInstructions,

                // Default values for approval information
                ApprovalStatus = "",
                ApprovalNote = "",
                FridgeAllocation = fridgeAllocation
            };

            await SetLayoutData();
            await EmployeeNotification();
            await CustomerNotification();
            return View(model);
        }

        [HttpPost, ActionName("Process")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessConfirmed(int id, [Bind("FridgeAllocationId,ApprovalStatus,ApprovalNote")] ProcessAllocation processAllocation)
        {
            if (id != processAllocation.FridgeAllocationId)
            {
                return NotFound();
            }

            var existingAllocation = await _context.FridgeAllocations
                .Include(fa => fa.Customer)
                .Include(fa => fa.Fridge)
                .FirstOrDefaultAsync(fa => fa.FridgeAllocationId == id);

            if (existingAllocation == null)
            {
                return NotFound();
            }

            // Get EmployeeId based on the logged-in user
            var employeeId = await _context.Employees
                .Where(e => e.Email == User.Identity.Name)
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            // Validate EmployeeId
            if (employeeId == 0)
            {
                ModelState.AddModelError("", "Employee ID is not valid.");
                ViewData["FridgeId"] = new SelectList(_context.Fridges.Where(f => !f.IsScrapped), "FridgeId", "SerialNumber", existingAllocation.FridgeId);
                return View(processAllocation); // Return to the view with the error
            }

            // Ensure processAllocation is not null and its properties are valid
            if (processAllocation == null)
            {
                return BadRequest("ProcessAllocation cannot be null");
            }

            existingAllocation.IsProcessed = true;

            var newProcessAllocation = new ProcessAllocation
            {
                FridgeAllocationId = processAllocation.FridgeAllocationId,
                CustomerId = existingAllocation.CustomerId, // Ensure this is set
                CustomerName = existingAllocation.Customer?.FirstName ?? "Unknown",
                CustomerLast = existingAllocation.Customer?.LastName ?? "Unknown",
                FridgeId = existingAllocation.FridgeId,
                SerialNumber = existingAllocation.SerialNumber,
                AllocationDate = existingAllocation.AllocationDate,
                SpecialInstructions = existingAllocation.SpecialInstructions,
                DeliveryPickupDate = processAllocation.DeliveryPickupDate,
                ApprovalStatus = processAllocation.ApprovalStatus,
                ApprovalNote = processAllocation.ApprovalNote,
                EmployeeId = employeeId,
                LastModifiedBy = User.Identity.Name,
                LastModifiedDate = DateTime.Now,
                FridgeAllocation = existingAllocation
            };

            _context.ProcessAllocations.Add(newProcessAllocation);
            _context.FridgeAllocations.Update(existingAllocation);

            // Fetch the customer for notification
            var customer = existingAllocation.Customer;

            // Create a notification for processing the fridge allocation
            var notificationMessage = $"Fridge {existingAllocation.SerialNumber} has been processed.";
            var notification = new Notification
            {
                Message = notificationMessage,
                ActionBy = User.Identity.Name,
                EmployeeId = employeeId,
                Date = DateTime.Now
            };

            // Add notification to the database for administrators and customer liaisons
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // Fetch all administrators and customer liaisons to notify
            var relevantEmployees = await _context.Employees
                .Where(e => e.Role == "Administrator" || e.Role == "Customer Liaison" || e.Role == "Inventory Liaison")
                .ToListAsync();

            // Create EmployeeNotificationStatus for all relevant employees
            foreach (var employee in relevantEmployees)
            {
                var status = new EmployeeNotificationStatus
                {
                    EmployeeId = employee.EmployeeId,
                    NotificationId = notification.Id,
                    IsRead = employee.Email == User.Identity.Name // Mark as read for the creator
                };

                _context.EmployeeNotificationStatuses.Add(status);
            }

            // Send notification to the customer
            var customerNotification = new Notification
            {
                Message = $"Your fridge allocation with serial number {existingAllocation.SerialNumber} has been processed.",
                ActionBy = User.Identity.Name,
                EmployeeId = employeeId, // or any relevant employee ID
                Date = DateTime.Now
            };

            // Add the customer notification to the database
            await _context.Notifications.AddAsync(customerNotification);
            await _context.SaveChangesAsync();

            // Save all changes
            await _context.SaveChangesAsync();

            TempData["processed"] = "Fridge Allocation processed successfully!";
            return RedirectToAction(nameof(Index));
        }


        // GET: ProcessAllocation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processAllocation = await _context.ProcessAllocations
                .Include(pa => pa.FridgeAllocation)
                .ThenInclude(fa => fa.Customer)
                .Include(pa => pa.FridgeAllocation)
                .ThenInclude(fa => fa.Fridge)
                .FirstOrDefaultAsync(m => m.ProcessAllocationId == id);
            if (processAllocation == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            await EmployeeNotification();
            await CustomerNotification();
            return View(processAllocation);
        }

        // Check if ProcessAllocation record exists
        private bool ProcessAllocationExists(int id)
        {
            return _context.ProcessAllocations.Any(e => e.ProcessAllocationId == id);
        }
    }
}