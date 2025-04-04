using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ont3010_Project_YA2024.Controllers.CustomerLiaison;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.Purchasing_Manager
{
    public class PurchasingManagerController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchasingManagerController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public PurchasingManagerController(BusinessService businessService, ApplicationDbContext context
            , ILogger<PurchasingManagerController> logger, UserManager<IdentityUser> userManager, INotificationService notificationService)
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
