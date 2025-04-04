using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.admin;

namespace Ont3010_Project_YA2024.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly NotificationService _notificationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public NotificationsController(BusinessService businessService, UserManager<IdentityUser> userManager
            , ApplicationDbContext context, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _userManager = userManager;
            _context = context;
        }


        public IActionResult index()
        {
           
            return View();
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
            else if (User.IsInRole("Inventory Liaison"))
            {
                ViewData["Layout"] = "~/Views/Shared/_InventoryLiaisonLayout.cshtml";
            }
            else if (User.IsInRole("Purchasing Manager"))
            {
                ViewData["Layout"] = "~/Views/Shared/_PurchasingManagerLayout.cshtml";
            }
            else if (User.IsInRole("Maintenance Technician"))
            {
                ViewData["Layout"] = "~/Views/Shared/_MaintenanceTechnicianLayout.cshtml";
            }
            else if (User.IsInRole("Fault Technician"))
            {
                ViewData["Layout"] = "~/Views/Shared/_FaultTechnicianLayout.cshtml";
            }
            else if (User.IsInRole("Supplier"))
            {
                ViewData["Layout"] = "~/Views/Shared/_SupplierLayout.cshtml";
            }
            else if (User.IsInRole("Customer"))
            {
                ViewData["Layout"] = "~/Views/Shared/_CustomerLayout.cshtml";
            }
            else
            {
                ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml"; // Default layout
            }
        }

        public async Task<IActionResult> AllNotifications()
        {
            await SetLayoutData();
            SetLayoutBasedOnRole();
            await CustomerNotification();
            await EmployeeNotification();
            var userEmail = User.Identity.Name;

            // Retrieve the employee details using the email
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            // Ensure that the employee exists
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            // Check if the employee is an administrator
            bool isAdmin = employee.Role == "Administrator";

            // Fetch all notifications for the logged-in user or administrator
            var notifications = await _context.Notifications
                .Include(n => n.EmployeeNotificationStatuses) // Include notification statuses
                .Where(n =>
                    n.ActionBy == userEmail || // Fetch notifications created by the user
                    isAdmin || // Fetch all notifications for admins
                    n.EmployeeNotificationStatuses.Any(s => s.EmployeeId == employee.EmployeeId)) // Or any notification targeting the logged-in user
                .OrderByDescending(n => n.Date)
                .ToListAsync();

            // Pass the employee ID to the view
            ViewBag.EmployeeId = employee.EmployeeId;

            return View(notifications);
        }

    }
}