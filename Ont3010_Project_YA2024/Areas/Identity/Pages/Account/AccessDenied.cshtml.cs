// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Mvc.RazorPages;
using Ont3010_Project_YA2024.Data.Helpers;

namespace Ont3010_Project_YA2024.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        private readonly BusinessService _businessService; // Ensure this is the correct service for your context

        public AccessDeniedModel(BusinessService businessService)
        {
            _businessService = businessService;
        }

        public void OnGet()
        {
            // Call SetLayoutData on GET request
            SetLayoutData().Wait(); // Be cautious with .Wait(), consider async call if possible
        }

        private async Task SetLayoutData()
        {
            var business = await _businessService.GetBusinessAsync();
            var setting = await _businessService.GetSettingAsync();

            ViewData["BusinessName"] = setting?.BusinessName ?? "Default Name";
            ViewData["ContactEmail"] = setting?.ContactEmail ?? "default@example.com";
            ViewData["ContactPhone"] = setting?.ContactPhone ?? "000-000-0000";
            ViewData["Address"] = $"{setting?.Business.Street}, {setting?.Business.City}, {setting?.Business.StateProvince}, {setting?.Business.Country}"; // if iwant to use the whole address
            ViewData["Street"] = setting?.Business.Street ?? "Default Street";
            ViewData["City"] = setting?.Business.City ?? "Default City";
            ViewData["StateProvince"] = setting?.Business.StateProvince ?? "Default Province";
            ViewData["Country"] = setting?.Business.Country ?? "Default Country";
            ViewData["Slogan"] = business?.Slogan ?? "Your business slogan goes here.";

            if (setting?.BusinessLogo != null)
            {
                string businessLogoBase64 = Convert.ToBase64String(setting.BusinessLogo);
                ViewData["BusinessLogo"] = $"data:image/jpeg;base64,{businessLogoBase64}";
            }
            else
            {
                ViewData["BusinessLogo"] = "/path/to/default/logo.png"; // Provide a default logo path if needed
            }

            if (setting?.CoverPhoto != null)
            {
                string coverPhotoBase64 = Convert.ToBase64String(setting.CoverPhoto);
                ViewData["CoverPhoto"] = $"data:image/jpeg;base64,{coverPhotoBase64}";
            }
            else
            {
                ViewData["CoverPhoto"] = "/path/to/default/cover/photo.jpg"; // Provide a default cover photo path if needed
            }
        }
    }
}
