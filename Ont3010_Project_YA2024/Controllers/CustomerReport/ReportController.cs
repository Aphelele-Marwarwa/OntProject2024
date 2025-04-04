using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Models.CustomerReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNet.Identity;
using Ont3010_Project_YA2024.Data.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.CustomerReport
{
    [Authorize(Roles = "Customer")]
    public class ReportController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ReportController(BusinessService businessService, ApplicationDbContext context, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _context = context;
        }
       
        private int? GetCurrentCustomerId()
        {
            string currentUserEmail = User.Identity.Name;
            var isTechnician = _context.Employees.Any(e => e.Email == currentUserEmail);

            if (isTechnician)
            {
                // If the user is a Fault Technician, return null
                return null; 
            }
            var customer = _context.Customers
                                    .FirstOrDefault(c => c.EmailAddress == currentUserEmail);

         
            return customer?.CustomerId; 
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await SetLayoutData();
            var customerId = GetCurrentCustomerId();

            if (customerId == null)
            {
                TempData["Error"] = "Unauthorized access or Customer not found. Please log in.";
                return RedirectToAction("Index");
            }

            var fridges = _context.ProcessAllocations
                .Where(fa => fa.CustomerId == customerId.Value && fa.ApprovalStatus == "Approved")
                  .Select(fa => new
                   {
                    fa.Fridge.FridgeId,
                    fa.Fridge.SerialNumber,
                    fa.Fridge.ModelType,
                    fa.Fridge.WarrantyEndDate,
                    fa.ApprovalStatus
                    })
                    .ToList();

            // Pass the fridges via ViewBag
            ViewBag.Fridges = fridges;
          
            return View(); // Pass the Report model to the view
        }
       





        // GET: IndexNewRequest
       
    }
}
