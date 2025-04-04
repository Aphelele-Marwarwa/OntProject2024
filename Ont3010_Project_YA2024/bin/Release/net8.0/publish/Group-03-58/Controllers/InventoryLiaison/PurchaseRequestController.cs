using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.InventoryLiaison;

namespace Ont3010_Project_YA2024.Controllers.InventoryLiaison
{
    [Authorize(Roles = "Inventory Liaison")]
    public class PurchaseRequestController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PurchaseRequestController(BusinessService businessService,ApplicationDbContext context, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _context = context;
        }

        // GET: PurchaseRequest
        public async Task<IActionResult> Index(int page = 1)
        {
            await SetLayoutData();
            await EmployeeNotification();

            // Define the page size
            int pageSize = 10;

            // Get the total number of purchase requests from the database
            var totalAllocations = await _context.PurchaseRequests.CountAsync();

            // Retrieve the paginated list of purchase requests
            var pagedList = await _context.PurchaseRequests
                .Include(p => p.Fridge) // Include the Fridge navigation property
                .Skip((page - 1) * pageSize) // Skip the items for the previous pages
                .Take(pageSize) // Take the items for the current page
                .ToListAsync();

            // Set pagination data in ViewBag
            ViewBag.TotalPages = (int)Math.Ceiling(totalAllocations / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedList); // Pass the paginated list to the view
        }


        // GET: PurchaseRequest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            await EmployeeNotification();
            var purchaseRequest = await _context.PurchaseRequests
                .Include(p => p.Fridge) // Include the Fridge navigation property
                .FirstOrDefaultAsync(m => m.PurchaseRequestId == id);
            if (purchaseRequest == null)
            {
                return NotFound();
            }

            return View(purchaseRequest);
        }

        // GET: PurchaseRequest/Create
        public async Task <IActionResult> Create()
        {
            ViewData["FridgeId"] = new SelectList(_context.Fridges.Where(f => f.IsInStock), "FridgeId", "ModelType");
            await SetLayoutData();
            return View();
        }

        // POST: PurchaseRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseRequest purchaseRequest)
        {
            if (!ModelState.IsValid)
            {
                var employeeId = _context.Employees
             .Where(e => e.Email == User.Identity.Name)
             .Select(e => e.EmployeeId)
             .FirstOrDefault();

                if (employeeId <= 0)
                {
                    ModelState.AddModelError("", "Employee ID is not valid.");
                    ViewData["FridgeId"] = new SelectList(_context.Fridges.Where(f => f.IsInStock), "FridgeId", "ModelType", purchaseRequest.FridgeId);
                    await SetLayoutData();
                    return View(purchaseRequest);
                }

                purchaseRequest.EmployeeId = employeeId;
                purchaseRequest.CreatedBy = User.Identity.Name;
                purchaseRequest.CreatedDate = DateTime.Now;
                _context.Add(purchaseRequest);
                await _context.SaveChangesAsync();
                TempData["created"] = "Purchase request created successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "There was an error creating the purchase request. Please check the form and try again.";
            ViewData["FridgeId"] = new SelectList(_context.Fridges.Where(f => f.IsInStock), "FridgeId", "ModelType", purchaseRequest.FridgeId);
            await SetLayoutData();
            await EmployeeNotification();
            return View(purchaseRequest);
        }

       
        private bool PurchaseRequestExists(int id)
        {
            return _context.PurchaseRequests.Any(e => e.PurchaseRequestId == id);
        }
    }
}
