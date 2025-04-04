

namespace Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices
{
    public class FridgeReportData
    {
        public int FridgeId { get; set; }
        public string SerialNumber { get; set; }
        public string ModelType { get; set; }
        public string Condition { get; set; }
        public string DoorType { get; set; } // New property
        public string Size { get; set; } // New property
        public string Capacity { get; set; } // New property
        public string SupplierName { get; set; } // New property
        public string SupplierContact { get; set; } // New property
        public bool IsInStock { get; set; } // New property
        public DateTime? WarrantyEndDate { get; set; } // New property
        public DateTime CreatedDate { get; set; }

    }
}
