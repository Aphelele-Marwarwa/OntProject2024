using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
 

        // Define roles to be created
        var roles = new List<string>
        {
            "Administrator",
            "Customer Liaison",
            "Inventory Liaison",
            "Purchasing Manager",
            "Maintenance Technician",
            "Fault Technician",
            "Supplier",
            "Customer"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

    }
}
