using Ont3010_Project_YA2024.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ont3010_Project_YA2024.Data.CustomerLiaisonRepServices;
using Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using OfficeOpenXml;

using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("conn");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password options
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedAccount = true; // Set to true if you want to enforce email confirmation

    // Lockout options
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User options
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
})
.AddRoles<IdentityRole>()  // Add roles service
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<PdfService>();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
builder.Services.AddScoped<GenerateFridgeReportPdf>();
builder.Services.AddScoped<GenerateFridgeReportExcel>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddTransient<ExcelService>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression();
builder.Services.AddTransient<BusinessService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Ensure the required roles and initial users exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        // Log errors or handle them as needed
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Index}/{id?}",
    defaults: new { controller = "AdminDashboard" });

app.MapControllerRoute(
    name: "customerLiaison",
    pattern: "CustomerLiaison/{action=Index}/{id?}",
    defaults: new { controller = "CustomerLiaison" });

app.MapControllerRoute(
    name: "inventoryLiaison",
    pattern: "InventoryLiaison/{action=Index}/{id?}",
    defaults: new { controller = "InventoryLiaison" });
app.MapControllerRoute(
    name: "fridgeDetails",
    pattern: "Fridge/Details/{id}/{serialNumber?}",
    defaults: new { controller = "Fridge", action = "Details" });

app.MapControllerRoute(
    name: "fridgeEdit",
    pattern: "Fridge/Edit/{id}/{serialNumber?}",
    defaults: new { controller = "Fridge", action = "Edit" });

app.MapControllerRoute(
    name: "fridgeDelete",
    pattern: "Fridge/Delete/{id}/{serialNumber?}",
    defaults: new { controller = "Fridge", action = "Delete" });

app.MapControllerRoute(
    name: "Fault Technician",
    pattern: "Fault Technician/{action=Index}/{id?}",
    defaults: new { controller = "Fault Technician" });
app.MapControllerRoute(
    name: "Customer",
    pattern: "Customer/{action=Index}/{id?}",
    defaults: new { controller = "Customer" });
app.MapControllerRoute(
    name: "Maintenance Technician",
    pattern: "Maintenance Technician/{action=Index}/{id?}",
    defaults: new { controller = "Maintenance Technician" });
app.MapControllerRoute(
    name: "Purchasing Manager",
    pattern: "Purchasing Manager/{action=Index}/{id?}",
    defaults: new { controller = "Purchasing Manager" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();
