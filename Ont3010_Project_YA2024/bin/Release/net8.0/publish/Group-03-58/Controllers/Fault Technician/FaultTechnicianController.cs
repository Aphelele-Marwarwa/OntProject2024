using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ont3010_Project_YA2024.Controllers.Purchasing_Manager;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.Fault_Technician
{
    public class FaultTechnicianController : BaseController 
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FaultTechnicianController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public FaultTechnicianController(BusinessService businessService, 
            ApplicationDbContext context, ILogger<FaultTechnicianController> logger, UserManager<IdentityUser> userManager, INotificationService notificationService)
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
