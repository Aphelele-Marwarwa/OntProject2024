﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ont3010_Project_YA2024.Controllers.Maintenance_Technician;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Data;

namespace Ont3010_Project_YA2024.Controllers.Supplier
{
    public class SupplierController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MaintenanceTechnicianController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public SupplierController(BusinessService businessService, ApplicationDbContext context
            , ILogger<MaintenanceTechnicianController> logger, UserManager<IdentityUser> userManager, INotificationService notificationService)
            : base(businessService, context, notificationService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            await SetLayoutData();
            return View();
        }
    }
}
