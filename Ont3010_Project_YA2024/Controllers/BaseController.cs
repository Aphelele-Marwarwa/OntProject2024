using Microsoft.AspNetCore.Mvc;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.admin;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using Ont3010_Project_YA2024.Models;

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
    protected async Task CreateEmployeeNotificationAsync(string action, Employee targetEmployee, string message)
    {
        // Step 1: Get the ActionBy (the person who performed the action)
        var actionByEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == User.Identity.Name);

        var actionBy = actionByEmployee != null ?
            $"{actionByEmployee.FirstName} {actionByEmployee.LastName}" :
            User.Identity.Name; // Fallback to email if name is not found

        // Step 2: Create a notification
        var notification = new Notification
        {
            Message = message,
            ActionBy = actionBy,
            EmployeeId = targetEmployee.EmployeeId,
            Date = DateTime.Now
        };

        // Step 3: Add notification to database
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Step 4: Fetch all administrators
        var administrators = await _context.Employees
            .Where(e => e.Role == "Administrator")
            .ToListAsync();

        // Step 5: Create EmployeeNotificationStatus for all administrators
        foreach (var admin in administrators)
        {
            var status = new EmployeeNotificationStatus
            {
                EmployeeId = admin.EmployeeId,
                NotificationId = notification.Id,
                IsRead = false // Set IsRead to false by default for all admins
            };

            // Mark as read if the admin is the one who performed the action
            if (admin.Email == User.Identity.Name)
            {
                status.IsRead = true;
            }

            _context.EmployeeNotificationStatuses.Add(status);
        }

        // Step 6: Save changes for notification statuses
        await _context.SaveChangesAsync();
    }


    protected async Task CreateCustomerNotificationAsync(string action, Customer targetCustomer, string message)
    {
        // Step 1: Get the ActionBy (the person who performed the action)
        var actionByCustomer = await _context.Customers
            .FirstOrDefaultAsync(c => c.EmailAddress == User.Identity.Name);

        var actionBy = actionByCustomer != null ?
            $"{actionByCustomer.FirstName} {actionByCustomer.LastName}" :
            User.Identity.Name; // Fallback to email if name is not found

        // Step 2: Create a notification
        var notification = new Notification
        {
            Message = message,
            ActionBy = actionBy,
            CustomerId = targetCustomer.CustomerId, // Target customer
            Date = DateTime.Now
        };

        // Step 3: Add notification to the database
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Step 4: Fetch all employees with the role of 'Administrator' (or Customer Liaisons)
        var administrators = await _context.Employees // Fetch from Employees table
            .Where(e => e.Role == "Administrator") // Check for "Administrator" role
            .ToListAsync();

        // Step 5: Create EmployeeNotificationStatus for all administrators
        foreach (var admin in administrators)
        {
            var status = new EmployeeNotificationStatus // Assuming this tracks notifications for employees
            {
                EmployeeId = admin.EmployeeId, // Employee to notify
                NotificationId = notification.Id,
                IsRead = false // Set IsRead to false by default for all admins
            };

            // Mark as read if the admin is the one who performed the action
            if (admin.Email == User.Identity.Name)
            {
                status.IsRead = true;
            }

            _context.EmployeeNotificationStatuses.Add(status); // Add status for admins
        }

        // Step 6: Notify the target customer as well
        var customerNotificationStatus = new EmployeeNotificationStatus
        {
            CustomerId = targetCustomer.CustomerId, // Notify the specific customer
            NotificationId = notification.Id,
            IsRead = false // Set IsRead to false for the target customer
        };

        _context.EmployeeNotificationStatuses.Add(customerNotificationStatus); // Add status for the target customer

        // Step 7: Save changes for notification statuses
        await _context.SaveChangesAsync();
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


        // Fetch unread notifications for the logged-in customer
        var notifications = await _context.EmployeeNotificationStatuses
            .Include(cns => cns.Notification) // Include the Notification entity
            .Where(cns => cns.EmployeeId == employee.EmployeeId && !cns.IsRead) // Filter for unread notifications
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
    protected async Task CustomerNotification()
    {
        // Check if the user is authenticated and their identity exists
        var userEmail = User?.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            // Handle case where the user is not authenticated or email is unavailable
            ViewBag.Notifications = new List<EmployeeNotificationStatus>();
            ViewBag.UnreadNotificationCount = 0;
            ViewBag.CustomerId = null;
            return;
        }

        // Fetch the customer based on the user email
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == userEmail);
        if (customer == null)
        {
            // Handle case where the customer is not found
            ViewBag.Notifications = new List<EmployeeNotificationStatus>();
            ViewBag.UnreadNotificationCount = 0;
            ViewBag.CustomerId = null;
            return;
        }

        // Fetch unread notifications for the logged-in customer
        var notifications = await _context.EmployeeNotificationStatuses
            .Include(cns => cns.Notification) // Include the Notification entity
            .Where(cns => cns.CustomerId == customer.CustomerId && !cns.IsRead) // Filter for unread notifications
            .ToListAsync();
        // Count unread notifications for the customer
        var unreadNotificationCount = notifications?.Count(n => !n.IsRead) ?? 0;

        // Pass data to ViewBag
        ViewBag.Notifications = notifications;
        ViewBag.UnreadNotificationCount = unreadNotificationCount;
        ViewBag.CustomerId = customer.CustomerId;

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
