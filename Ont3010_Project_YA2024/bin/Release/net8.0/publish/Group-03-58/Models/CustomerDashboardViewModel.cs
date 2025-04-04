using Ont3010_Project_YA2024.Models.CustomerLiaison;

namespace Ont3010_Project_YA2024.Models
{
    public class CustomerDashboardViewModel
    {
        public Customer Customer { get; set; }
        public List<FridgeAllocation> RecentFridgeAllocations { get; set; }
        public int TotalAllocations { get; set; }
        public int ProcessedAllocations { get; set; }
        public int UnprocessedAllocations { get; set; }
        public AllocationGraphData AllocationGraphData { get; set; }
    }

    public class AllocationGraphData
    {
        public int Processed { get; set; }
        public int Unprocessed { get; set; }
    }
}
