using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using Ont3010_Project_YA2024.Models.InventoryLiaison;

namespace Ont3010_Project_YA2024.Controllers.CustomerLiaison
{
    [Authorize(Roles = "Administrator, Customer Liaison")]
    public class FridgeAllocationController : BaseController
    {

        private readonly ApplicationDbContext _context;

        public FridgeAllocationController(BusinessService businessService, ApplicationDbContext context, INotificationService notificationService)
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
            else if (User.IsInRole("Customer Liaison"))
            {
                ViewData["Layout"] = "~/Views/Shared/_CustomerLiaisonLayout.cshtml";
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
        public async Task<IActionResult> Index(string searchString, string sortOrder, string sortDirection, DateTime? allocationDate, int page = 1)
        {
            SetLayoutBasedOnRole();
            await EmployeeNotification();
        
            await SetLayoutData();

            // Debug log to check if searchString is being received
            Console.WriteLine($"Search String: '{searchString}'");

            // Set default sorting values
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "asc" : sortDirection;
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "SerialNumber" : sortOrder;

            // Set ViewData for current sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentDirection"] = sortDirection;
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentAllocationDate"] = allocationDate;

            // Retrieve allocations with search and sorting applied
            var allocations = _context.FridgeAllocations
                .Include(f => f.Customer)
                .Include(f => f.Fridge)
                .AsQueryable();

            // Check if searchString is null or empty
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchTerms = searchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Update the allocation query to search for full names
                allocations = allocations.Where(a =>
                    a.Customer.FirstName.Contains(searchString) ||
                    a.Customer.LastName.Contains(searchString) ||
                    (searchTerms.Length > 1 &&
                     searchTerms.All(term =>
                         a.Customer.FirstName.Contains(term) ||
                         a.Customer.LastName.Contains(term))) ||
                    (searchTerms.Length == 1 &&
                     (a.Customer.FirstName.Contains(searchTerms[0]) ||
                      a.Customer.LastName.Contains(searchTerms[0]))) ||
                    a.Fridge.FridgeId.ToString().Contains(searchString) // Optional: Search by Fridge ID
                );
            }


            // Filter by allocation date if provided
            if (allocationDate.HasValue)
            {
                allocations = allocations.Where(a => a.AllocationDate.Date == allocationDate.Value.Date);
            }

            // Apply sorting
            allocations = SortFridgeAllocations(allocations, sortOrder, sortDirection);

            // Pagination
            int pageSize = 10;
            var totalAllocations = await allocations.CountAsync();
            var pagedList = await allocations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalAllocations / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedList);
        }


        private IQueryable<FridgeAllocation> SortFridgeAllocations(IQueryable<FridgeAllocation> allocations, string sortOrder, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            return sortOrder.ToLower() switch
            {
                "serialnumber" => isAscending ? allocations.OrderBy(a => a.Fridge.SerialNumber) : allocations.OrderByDescending(a => a.Fridge.SerialNumber),
                "fridgeid" => isAscending ? allocations.OrderBy(a => a.Fridge.FridgeId) : allocations.OrderByDescending(a => a.Fridge.FridgeId),
                "allocationdate" => isAscending ? allocations.OrderBy(a => a.AllocationDate) : allocations.OrderByDescending(a => a.AllocationDate),
                "customerfirstname" => isAscending ? allocations.OrderBy(a => a.Customer.FirstName) : allocations.OrderByDescending(a => a.Customer.FirstName),
                "customerlastname" => isAscending ? allocations.OrderBy(a => a.Customer.LastName) : allocations.OrderByDescending(a => a.Customer.LastName),
                _ => isAscending ? allocations.OrderBy(a => a.Fridge.SerialNumber) : allocations.OrderByDescending(a => a.Fridge.SerialNumber)
            };
        }

        // GET: FridgeAllocation/Create
        public async Task<IActionResult> Create(int? id)
        {
            // Fetch customers and combine first and last name
            var customers = await _context.Customers
                .Where(c => !c.IsDeleted)
                .Select(c => new
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FirstName + " " + c.LastName // Combine first and last name
                })
                .ToListAsync();

            // Fetch available fridges
            var fridges = await _context.Fridges
                .Where(f => !f.IsScrapped && !f.IsDeleted && !f.IsAllocated && f.IsInStock)
                .ToListAsync();

            // Pass customer data for select dropdown
            ViewBag.CustomerList = new SelectList(customers, "CustomerId", "FullName");

            // Set fridges for dropdown
            ViewBag.Fridges = fridges.Select(f => new
            {
                f.FridgeId,
                f.SerialNumber,
                FridgeName = f.ModelType // Make sure this property exists in your Fridge model
            }).ToList();

            // Fetch full name and ID for the selected customer from URL
            if (id.HasValue)
            {
                var selectedCustomer = customers.FirstOrDefault(c => c.CustomerId == id.Value);
                if (selectedCustomer == null)
                {
                    ModelState.AddModelError("", "Customer not found.");
                    return View(); // Return early if customer not found
                }

                ViewBag.CustomerFullName = selectedCustomer.FullName;
                ViewBag.CustomerId = selectedCustomer.CustomerId; // Pass CustomerId for hidden field
            }

            SetLayoutBasedOnRole();
            await EmployeeNotification();
        
            await SetLayoutData();
            return View();
        }

        // POST: FridgeAllocation/Create
        // POST: FridgeAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FridgeAllocation fridgeAllocation)
        {
            // Fetch available fridges again to populate dropdown in case of an error
            var fridges = await _context.Fridges
                .Where(f => !f.IsScrapped && !f.IsDeleted && !f.IsAllocated && f.IsInStock)
                .Select(f => new
                {
                    f.FridgeId,
                    f.SerialNumber,
                    FridgeName = f.ModelType
                })
                .ToListAsync();

            // Set ViewBag for dropdown
            ViewBag.Fridges = fridges;

            // Get EmployeeId based on the logged-in user
            var employeeId = _context.Employees
                .Where(e => e.Email == User.Identity.Name) // Fetch the employee based on email (username)
                .Select(e => e.EmployeeId)
                .FirstOrDefault();

            if (employeeId == 0)
            {
                ModelState.AddModelError("", "Employee ID is not valid.");
                SetLayoutBasedOnRole();
                await SetLayoutData();
                await EmployeeNotification();
            
                return View(fridgeAllocation); // Return view on error
            }

            // Fetch the customer based on CustomerId provided in fridgeAllocation
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == fridgeAllocation.CustomerId && !c.IsDeleted);

            if (customer == null)
            {
                ModelState.AddModelError("", "Invalid Customer ID.");
                SetLayoutBasedOnRole();
                await SetLayoutData();
               
                return View(fridgeAllocation); // Return view on error
            }

            // Set CustomerId in fridgeAllocation
            fridgeAllocation.CustomerId = customer.CustomerId;  // Ensure this is set to the valid CustomerId

   
            if (!ModelState.IsValid)
            {
                var fridge = await _context.Fridges
                    .SingleOrDefaultAsync(f => f.FridgeId == fridgeAllocation.FridgeId);

                if (fridge == null || fridge.IsScrapped)
                {
                    ModelState.AddModelError("", "Fridge is either scrapped or does not exist.");
                    SetLayoutBasedOnRole();
                    await SetLayoutData();
                    return View(fridgeAllocation); // Return view on error
                }

                // Set other properties in fridgeAllocation
                fridgeAllocation.SerialNumber = fridge.SerialNumber;
                fridgeAllocation.EmployeeId = employeeId;
                fridgeAllocation.AllocationDate = DateTime.Now;
                fridgeAllocation.IsProcessed = false;
                fridgeAllocation.CreatedDate = DateTime.Now;
                fridgeAllocation.CreatedBy = User.Identity.Name;

                // Add fridge allocation and update fridge status
                _context.FridgeAllocations.Add(fridgeAllocation);
                fridge.IsAllocated = true;
                fridge.IsInStock = false;
                _context.Fridges.Update(fridge);

                await _context.SaveChangesAsync();

                // Step 1: Create notification
                var actionByEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == User.Identity.Name);

                var actionBy = actionByEmployee != null
                    ? $"{actionByEmployee.FirstName} {actionByEmployee.LastName}"
                    : User.Identity.Name;

                var notification = new Notification
                {
                    Message = $"Fridge {fridgeAllocation.SerialNumber} was allocated to {customer.BusinessName}.",
                    ActionBy = actionBy, // Use the full name of the user who created the fridge allocation
                    EmployeeId = employeeId,
                    Date = DateTime.Now
                };

                // Add notification to database for administrators and customer liaisons
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                // Step 2: Fetch all administrators and customer liaisons
                var relevantEmployees = await _context.Employees
                    .Where(e => e.Role == "Administrator" || e.Role == "Customer Liaison" || e.Role == "Inventory Liaison")
                    .ToListAsync();

                // Step 3: Create EmployeeNotificationStatus for all administrators and customer liaisons
                foreach (var employee in relevantEmployees)
                {
                    var status = new EmployeeNotificationStatus
                    {
                        EmployeeId = employee.EmployeeId,
                        NotificationId = notification.Id,
                        IsRead = false // Set IsRead to false by default for all
                    };

                    // Mark as read if the employee is the one who created the fridge allocation
                    if (employee.Email == User.Identity.Name)
                    {
                        status.IsRead = true;
                    }

                    _context.EmployeeNotificationStatuses.Add(status);
                }

                // Save changes for notification statuses
                await _context.SaveChangesAsync();

                // Step 4: Send notification to the customer
                var customerNotification = new Notification
                {
                    Message = $"A fridge has been allocated to your business: {customer.BusinessName}. Fridge Serial Number: {fridgeAllocation.SerialNumber}.",
                    ActionBy = actionBy,
                    EmployeeId = employeeId, // or any relevant employee ID
                    Date = DateTime.Now
                };

                // Add the customer notification to the database
                await _context.Notifications.AddAsync(customerNotification);
                await _context.SaveChangesAsync();

                TempData["created"] = "Fridge Allocation created successfully!";
                return RedirectToAction(nameof(Index));
            }

            SetLayoutBasedOnRole();
            await SetLayoutData();
            await EmployeeNotification();
            
            return View(fridgeAllocation); // Return view with model errors
        }










        // GET: FridgeAllocation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FridgeAllocations == null)
            {
                return NotFound();
            }

            var fridgeAllocation = await _context.FridgeAllocations
                .Include(f => f.Customer)
                .Include(f => f.Fridge)
                .Include(f => f.ProcessAllocations)
                .Include(f => f.Employee) // Include the Employee entity to access its properties
                .FirstOrDefaultAsync(m => m.FridgeAllocationId == id);

            if (fridgeAllocation == null)
            {
                return NotFound();
            }

            // Initialize variables for employee name
            string allocatedBy = "Unknown";

            // Check if Employee is not null to get the full name
            if (fridgeAllocation.Employee != null)
            {
                allocatedBy = $"{fridgeAllocation.Employee.FirstName} {fridgeAllocation.Employee.LastName}";
            }

            // Set the ViewBag or Model for the allocatedBy to be displayed in the view
            ViewBag.AllocatedBy = allocatedBy;
            SetLayoutBasedOnRole();
            await SetLayoutData();
            await EmployeeNotification();
         
            return View(fridgeAllocation); // Return the original fridgeAllocation model, or you could pass a view model
        }




    }
}
