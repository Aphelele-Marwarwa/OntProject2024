using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.Threading.Tasks;

namespace Ont3010_Project_YA2024.Data.Helpers
{
    public class BusinessService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BusinessService> _logger;

        public BusinessService(ApplicationDbContext context, ILogger<BusinessService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Business?> GetBusinessAsync()
        {
            try
            {
                return await _context.Businesses.FirstOrDefaultAsync(b => b.BusinessId == 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business.");
                throw;
            }
        }

        public async Task<Setting?> GetSettingAsync()
        {
            try
            {
                return await _context.Settings.Include(s => s.Business).FirstOrDefaultAsync(s => s.SettingId == 1);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the setting.");
                throw;
            }
        }
        public async Task<List<Customer>> GetCustomerReportDataAsync(string startDate, string endDate)
        {
            // Parse the dates
            DateTime? parsedStartDate = null;
            DateTime? parsedEndDate = null;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out DateTime tempStartDate))
            {
                parsedStartDate = tempStartDate;
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out DateTime tempEndDate))
            {
                parsedEndDate = tempEndDate;
            }

            // Query the customers based on the date range
            var query = _context.Customers.AsQueryable();

            if (parsedStartDate.HasValue)
            {
                query = query.Where(c => c.CreatedDate >= parsedStartDate.Value);
            }

            if (parsedEndDate.HasValue)
            {
                query = query.Where(c => c.CreatedDate <= parsedEndDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<FridgeReportData>> GetFridgeReportDataAsync(string startDate, string endDate)
        {
            // Validate input dates
            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
            {
                _logger.LogWarning("Invalid date format: StartDate = {startDate}, EndDate = {endDate}", startDate, endDate);
                return Enumerable.Empty<FridgeReportData>(); // Return an empty collection if dates are invalid
            }

            try
            {
                return await _context.Fridges
                    .Where(f => f.CreatedDate >= start && f.CreatedDate <= end)
                    .Select(f => new FridgeReportData
                    {
                        FridgeId = f.FridgeId,
                        SerialNumber = f.SerialNumber,
                        ModelType = f.ModelType,
                        Condition = f.Condition,
                        DoorType = f.DoorType, // New property
                        Size = f.Size, // New property
                        Capacity = f.Capacity, // New property
                        SupplierName = f.SupplierName, // New property
                        SupplierContact = f.SupplierContact, // New property
                        IsInStock = f.IsInStock, // New property
                        WarrantyEndDate = f.WarrantyEndDate,
                        CreatedDate = f.CreatedDate// New property
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error retrieving fridge report data for dates {startDate} to {endDate}.", startDate, endDate);
                throw; // Rethrow or handle as needed
            }
        }

    }
}
