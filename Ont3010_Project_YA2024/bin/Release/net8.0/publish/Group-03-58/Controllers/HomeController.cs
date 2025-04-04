using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using System.Diagnostics;

namespace Ont3010_Project_YA2024.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        public HomeController(BusinessService businessService, ApplicationDbContext context, ILogger<HomeController> logger, IEmailSender emailSender, INotificationService notificationService)
         : base(businessService, context, notificationService) // Provide all required parameters here
        {
            _logger = logger;
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
        public async Task<IActionResult> Index()
        {
            await SetLayoutData(); // Set layout data
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            await SetLayoutData();
            return View();
        }
        public async Task<IActionResult> About()
        {
            await SetLayoutData();
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            await SetLayoutData(); // Assuming this is used to set layout-specific data
            return View();
        }

        // This method will handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string name, string email, string message)
        {
            if (ModelState.IsValid)
            {
                // Send an email to the admin
                await _emailSender.SendEmailAsync(
                    "marwarwa.ap@gmail.com",   // Admin email address
                    "New Contact Form Submission",    // Email subject
                    $"Name: {name}<br>Email: {email}<br>Message: {message}" // Email body
                );

                // Send a confirmation email to the user
                await _emailSender.SendEmailAsync(
                    email,  // User's email
                    "Thank you for contacting us",  // Subject
                    $"Hi {name},<br>Thank you for reaching out to us. We have received your message and will respond shortly." // Body
                );

                await SetLayoutData();
                return RedirectToAction("ThankYou");
            }

            await SetLayoutData();
            return View("Contact");
        }


        // This method displays a Thank You message
        public async Task<IActionResult> ThankYou()
        {
            await SetLayoutData(); // Ensure layout data is set before rendering the view
            return View(); // Render the Contact view
        }


        public async Task<IActionResult> FAQs()
        {
            await SetLayoutData();
            return View();
        }
        public async Task<IActionResult> Products(int pageNumber = 1, int pageSize = 10)
        {
            await SetLayoutData();
            SetLayoutBasedOnRole();
            // Fetch fridges that are in stock, not scrapped, not deleted, and not allocated
            var fridgesQuery = _context.Fridges
                .Where(f => f.IsInStock && !f.IsScrapped && !f.IsDeleted && !f.IsAllocated);

            // Calculate total number of items
            var totalFridges = await fridgesQuery.CountAsync();

            // Fetch the current page of fridges
            var fridges = await fridgesQuery
                .OrderBy(f => f.ModelType) // Optional: Adjust ordering as needed
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Create ViewModel
            var viewModel = new PagedFridgeViewModel
            {
                Fridges = fridges,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalFridges / (double)pageSize)
            };
            SetLayoutBasedOnRole();
            return View(viewModel);
        }



        public IActionResult Search(string searchQuery)
        {
            // Logic to filter fridges based on searchQuery
            var fridges = _context.Fridges
                .Where(f => f.ModelType.Contains(searchQuery) || f.DoorType.Contains(searchQuery))
                .ToList();
            return View("Products", fridges); // Ensure "Products" is the view name where fridges are displayed
        }

        public IActionResult Sort(string sortBy)
        {
            // Logic to sort fridges based on sortBy
            IQueryable<Fridge> fridges = _context.Fridges;

            switch (sortBy)
            {
                case "ModelType":
                    fridges = fridges.OrderBy(f => f.ModelType);
                    break;
                case "Condition":
                    fridges = fridges.OrderBy(f => f.Condition);
                    break;
                case "Size":
                    fridges = fridges.OrderBy(f => f.Size);
                    break;
            }

            return View("Products", fridges.ToList()); // Ensure "Products" is the view name where fridges are displayed
        }

        public async Task<IActionResult> GetFridgeDetails(int id, string serialNumber)
        {
            var fridge = await _context.Fridges
                .FirstOrDefaultAsync(f => f.FridgeId == id && f.SerialNumber == serialNumber);

            if (fridge == null)
            {
                SetLayoutBasedOnRole();
                return PartialView("_Error"); // Optionally return an error partial view if fridge not found
            }
            SetLayoutBasedOnRole();
            return PartialView("_FridgeDetails", fridge); // Return a partial view with fridge details
        }


        public async Task<IActionResult> Services()
        {
            await SetLayoutData();
            return View();
        }

        public async Task<IActionResult> TermsOfService()
        {
            await SetLayoutData();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
