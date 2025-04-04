using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ont3010_Project_YA2024.Controllers
{
    public class TestEmailController : Controller
    {

        private readonly IEmailSender _emailSender;

        public TestEmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<IActionResult> SendTestEmail()
        {
            try
            {
                await _emailSender.SendEmailAsync("marwarwa.ap@gmail.com", "Test Subject", "This is a test email.");
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
