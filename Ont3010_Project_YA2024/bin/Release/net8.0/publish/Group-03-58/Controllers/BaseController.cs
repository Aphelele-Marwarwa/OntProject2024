using Microsoft.AspNetCore.Mvc;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.admin;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class BaseController : Controller
{
    protected readonly BusinessService _businessService;
    protected readonly ApplicationDbContext _context;
    protected readonly INotificationService _notificationService;

    public BaseController(BusinessService businessService, ApplicationDbContext context, INotificationService notificationService)
    {
        _businessService = businessService;
        _context = context;
        _notificationService = notificationService;
    }

    protected async Task EmployeeNotification()
    {
        // Check if the user is authenticated and their identity exists
        var userEmail = User?.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            // Handle case where the user is not authenticated or email is unavailable
            ViewBag.Notifications = new List<EmployeeNotificationStatus>();
            ViewBag.UnreadNotificationCount = 0;
            ViewBag.EmployeeId = null;
            return;
        }

        // Fetch the employee based on the user email
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);
        if (employee == null)
        {
            // Handle case where the employee is not found
            ViewBag.Notifications = new List<EmployeeNotificationStatus>();
            ViewBag.UnreadNotificationCount = 0;
            ViewBag.EmployeeId = null;
            return;
        }

        // Fetch all notifications for the logged-in employee
        var notifications = await _context.EmployeeNotificationStatuses
            .Include(ens => ens.Notification) // Include the Notification entity
            .Where(ens => ens.EmployeeId == employee.EmployeeId)
            .ToListAsync();

        // Count unread notifications for the employee
        var unreadNotificationCount = notifications?.Count(n => !n.IsRead) ?? 0;

        // Pass data to ViewBag
        ViewBag.Notifications = notifications;
        ViewBag.UnreadNotificationCount = unreadNotificationCount;
        ViewBag.EmployeeId = employee.EmployeeId;

        // Fetch business and settings (check if services are valid)
        if (_businessService != null)
        {
            var business = await _businessService.GetBusinessAsync();
            var setting = await _businessService.GetSettingAsync();

            // You can assign business and setting to ViewBag if needed
            ViewBag.Business = business;
            ViewBag.Setting = setting;
        }
        else
        {
            // Handle case where _businessService is not available
            ViewBag.Business = null;
            ViewBag.Setting = null;
        }
    }


    protected async Task SetLayoutData()
    {
        var business = await _businessService.GetBusinessAsync();
        var setting = await _businessService.GetSettingAsync();

        ViewBag.BusinessName = setting?.BusinessName ?? "Default Name";
        ViewBag.ContactEmail = setting?.ContactEmail ?? "default@example.com";
        ViewBag.ContactPhone = setting?.ContactPhone ?? "000-000-0000";
        ViewBag.Address = $"{setting?.Business.Street}, {setting?.Business.City},  {setting?.Business.PostalCode}, {setting?.Business.StateProvince}, {setting?.Business.Country}"; // if iwant to use the whole address
        ViewBag.Street = setting?.Business.Street ?? "Default Street";
        ViewBag.City = setting?.Business.City ?? "Default City";
        ViewBag.StateProvince = setting?.Business.StateProvince ?? "Default Province";
        ViewBag.Country = setting?.Business.Country ?? "Default Country";
        ViewBag.Slogan = business?.Slogan ?? "Your business slogan goes here.";

        if (setting?.BusinessLogo != null)
        {
            string businessLogoBase64 = Convert.ToBase64String(setting.BusinessLogo);
            ViewBag.BusinessLogo = $"data:image/jpeg;base64,{businessLogoBase64}";
        }
        else
        {
            ViewBag.BusinessLogo = "/path/to/default/logo.png"; // Provide a default logo path if needed
        }

        if (setting?.CoverPhoto != null)
        {
            string coverPhotoBase64 = Convert.ToBase64String(setting.CoverPhoto);
            ViewBag.CoverPhoto = $"data:image/jpeg;base64,{coverPhotoBase64}";
        }
        else
        {
            ViewBag.CoverPhoto = "/path/to/default/cover/photo.jpg"; // Provide a default cover photo path if needed
        }


        //var userName = User.Identity.Name;
        //if (!string.IsNullOrEmpty(userName))
        //{
        //    ViewBag.Notifications = await _notificationService.GetNotificationsForUserAsync(userName);
        //    ViewBag.UnreadNotificationCount = await _notificationService.GetUnreadNotificationCountAsync(userName);
        //}
        //else
        //{
        //    ViewBag.Notifications = null;
        //    ViewBag.UnreadNotificationCount = 0;
        //}
    }
}
