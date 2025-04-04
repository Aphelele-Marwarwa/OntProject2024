using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Models.admin;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.Admin
{
   // [Authorize(Roles = "Administrator")]
    public class SettingController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SettingController(BusinessService businessService, ApplicationDbContext context, INotificationService notificationService)
            : base(businessService, context,notificationService)
        {
            _context = context;
        }

        private bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".tif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024; // 5 MB size limit
        }

        // GET: Setting/Edit
        public async Task<IActionResult> Edit()
        {
            await SetLayoutData();
           await EmployeeNotification();
            try
            {
                var setting = await _context.Settings
                    .Include(s => s.Business)
                    .FirstOrDefaultAsync(s => s.SettingId == 1);

                if (setting == null)
                {
                    var business = await _context.Businesses.FirstOrDefaultAsync(b => b.BusinessId == 1);
                    if (business == null)
                    {
                        return NotFound();
                    }

                    setting = new Setting
                    {
                        BusinessId = 1,
                        BusinessName = "Default Business Name",
                        ContactEmail = "default@example.com",
                        ContactPhone = "000-000-0000",
                        BusinessLogo = new byte[0],
                        CoverPhoto = new byte[0],
                        Business = business
                    };

                    _context.Settings.Add(setting);
                    await _context.SaveChangesAsync();
                }

                return View(setting);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Setting model, IFormFile BusinessLogo, IFormFile CoverPhoto, string ExistingBusinessLogo, string ExistingCoverPhoto)
        {
            if (!ModelState.IsValid)
            {
                var setting = await _context.Settings.FindAsync(model.SettingId);

                if (setting != null)
                {
                    setting.BusinessName = model.BusinessName;
                    setting.ContactEmail = model.ContactEmail;
                    setting.ContactPhone = model.ContactPhone;
                    if (BusinessLogo != null && BusinessLogo.Length > 0 && !IsValidImage(BusinessLogo))
                    {
                        TempData["error"] = "Invalid Business Logo format. Allowed formats are .png, .jpg, .jpeg, .gif, .tif and size must be less than 5 MB.";
                        var existingSetting = await _context.Settings.FindAsync(model.SettingId);
                        if (existingSetting != null)
                        {
                            // Keep the existing values for display
                            model.BusinessLogo = existingSetting.BusinessLogo;
                            model.CoverPhoto = existingSetting.CoverPhoto;
                        }
                        await SetLayoutData();
                        return View(model);
                    }

                    if (CoverPhoto != null && CoverPhoto.Length > 0 && !IsValidImage(CoverPhoto))
                    {
                        TempData["error"] = "Invalid Cover Photo format. Allowed formats are .png, .jpg, .jpeg, .gif, .tif and size must be less than 5 MB.";
                        var existingSetting = await _context.Settings.FindAsync(model.SettingId);
                        if (existingSetting != null)
                        {
                            // Keep the existing values for display
                            model.BusinessLogo = existingSetting.BusinessLogo;
                            model.CoverPhoto = existingSetting.CoverPhoto;
                        }
                        await SetLayoutData();
                        return View(model);
                    }

                    if (BusinessLogo != null && BusinessLogo.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await BusinessLogo.CopyToAsync(ms);
                            setting.BusinessLogo = ms.ToArray();
                        }
                    }
                    else if (!string.IsNullOrEmpty(ExistingBusinessLogo))
                    {
                        setting.BusinessLogo = Convert.FromBase64String(ExistingBusinessLogo);
                    }

                    if (CoverPhoto != null && CoverPhoto.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await CoverPhoto.CopyToAsync(ms);
                            setting.CoverPhoto = ms.ToArray();
                        }
                    }
                    else if (!string.IsNullOrEmpty(ExistingCoverPhoto))
                    {
                        setting.CoverPhoto = Convert.FromBase64String(ExistingCoverPhoto);
                    }

                    setting.ModifiedBy = User.Identity.Name;
                    setting.ModifiedDate = DateTime.Now;

                    _context.Update(setting);
                    await _context.SaveChangesAsync();

                    // Call SetLayoutData after updating the setting
                    await SetLayoutData();

                    TempData["updated"] = "Settings updated successfully!";
                    return RedirectToAction(nameof(Edit));
                }
            }

            // Call SetLayoutData in case of model state errors as well
            await SetLayoutData();
            TempData["error"] = "Error updating settings. Please try again.";
            return View(model);
        }



        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.SettingId == id);
        }
    }
}
