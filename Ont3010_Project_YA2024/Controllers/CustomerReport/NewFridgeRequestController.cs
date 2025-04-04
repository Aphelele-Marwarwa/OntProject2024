using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Models.CustomerReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using iText.Layout.Renderer;

namespace Ont3010_Project_YA2024.Controllers.CustomerReport
{
    //[Authorize(Roles = "Customer")]
    public class NewFridgeRequestController : BaseController
    {
        private readonly ApplicationDbContext _context;
  
        public NewFridgeRequestController(BusinessService businessService, ApplicationDbContext context, INotificationService notificationService)
             : base(businessService, context, notificationService) 
        { 
            _context = context;
        }
        private int? GetCurrentCustomerId()
        {
            string currentUserEmail = User.Identity.Name;

            // Fetch the customer record from the database based on the current user's email
            var customer = _context.Customers
                                   .FirstOrDefault(c => c.EmailAddress == currentUserEmail);

           
            return customer?.CustomerId;
        }

        // GET: CreateNewRequest
        public async Task<IActionResult> CreateNewRequest()
        {
            await SetLayoutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewRequest(NewFridgeRequest fridgeRequest)
        {
            if (!ModelState.IsValid)
            {
                fridgeRequest.DateApplied = DateTime.Now; // Set the DateApplied to current date
                fridgeRequest.Status = "Pending"; // Set status to Pending

                // Get the current customer's ID
                int? currentCustomerId = GetCurrentCustomerId();
                if (currentCustomerId.HasValue)
                {
                    fridgeRequest.CustomerId = currentCustomerId.Value; 
                    _context.NewFridgeRequests.Add(fridgeRequest);
                    TempData["SuccessMessage"] = "Your fridge request was successfully created!";
                    await _context.SaveChangesAsync();
                    return RedirectToAction("IndexNewRequest"); 
                }
               
            }
                await SetLayoutData();
            return View(fridgeRequest);
        }

        // Action to view all New Fridge Requests for the logged-in customer
        public async Task<IActionResult> IndexNewRequest()
        {
            
            var customerId = GetCurrentCustomerId();

      
            if (!customerId.HasValue)
            {
                return Unauthorized("Customer ID not found. Please log in again."); 
            }

          
            var fridgeRequests = await _context.NewFridgeRequests
                .Where(r => r.CustomerId == customerId.Value)  
                .ToListAsync();

            if (fridgeRequests == null || !fridgeRequests.Any())
            {
                ViewBag.Message = "You have not made any fridge requests yet."; 
            }
            await SetLayoutData();
            return View(fridgeRequests);  
        }

      
        // Action to permanently delete a New Fridge Request
        [HttpPost]
        public async Task<IActionResult> DeleteNewFridgeRequest(int id)
        {
            var request = await _context.NewFridgeRequests.FindAsync(id);

          
            if (request != null && request.CustomerId == GetCurrentCustomerId())
            {
                _context.NewFridgeRequests.Remove(request); 
                await _context.SaveChangesAsync();

                TempData["Message"] = "The fridge request was successfully deleted.";
            }
            else
            {
                
                TempData["Error"] = "Error: Unable to delete the fridge request.";
            }
            await SetLayoutData();
            return RedirectToAction("IndexNewRequest");
        }
        public async Task<IActionResult> ViewRequestDetails(int id)
        {
            // Fetch the fridge request based on the ID
            var fridgeRequest = await _context.NewFridgeRequests
                .FirstOrDefaultAsync(r => r.NewFridgeRequestId == id);

            if (fridgeRequest == null)
            {
                return NotFound(); 
            }
            await SetLayoutData();
            return View(fridgeRequest); 
        }
    }
}
