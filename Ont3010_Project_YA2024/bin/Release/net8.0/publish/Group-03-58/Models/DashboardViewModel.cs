using Ont3010_Project_YA2024.Models.CustomerLiaison;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using Ont3010_Project_YA2024.Models.admin;

namespace Ont3010_Project_YA2024.Models
{
    public class DashboardViewModel
    {

        // Basic employee information
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Dashboard metrics
        public int TotalCustomers { get; set; }
        public int TotalFridges { get; set; }
        public int FridgesInStock { get; set; }
        public int FridgesScrapped { get; set; }
        public int FridgesAllocated { get; set; } // Add this property
        public int TotalPurchaseRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ProcessedAllocations { get; set; }
        public int PendingAllocations { get; set; }
        public int TotalEmployees { get; set; } // New property for total employees
        public List<int> Metrics { get; set; }
        // Additional data
        public IEnumerable<Notification> Notifications { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<PurchaseRequest> PurchaseRequests { get; set; }
        public List<FridgeAllocation> FridgeAllocations { get; set; }
        public List<Fridge> FridgesInStockList { get; set; }
        // Remove RecentActivities if not needed
        public List<FridgeAllocation> RecentActivities { get; set; } // Assuming FridgeAllocation as a placeholder
    }
}