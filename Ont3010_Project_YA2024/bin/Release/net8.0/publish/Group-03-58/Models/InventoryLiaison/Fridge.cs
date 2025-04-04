using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.ComponentModel.DataAnnotations;

namespace Ont3010_Project_YA2024.Models.InventoryLiaison
{
   
    public class Fridge
    {
        [Key]
        public int FridgeId { get; set; }

        
        [Required(ErrorMessage = "Fridge Model/Type is required.")]
        [StringLength(100, ErrorMessage = "Model/Type cannot be longer than 100 characters.")]
        [Display(Name = "Fridge Model/Type")]
        public string ModelType { get; set; } // Model or type of the fridge

        [Required(ErrorMessage = "Door Type is required.")]
        [StringLength(100)]
        public string DoorType { get; set; } 

        [Required(ErrorMessage = "Size is required.")]
        [StringLength(100)]
        public string Size { get; set; } 

        [Required(ErrorMessage = "Capacity is required.")]
        [StringLength(50)]
        public string Capacity { get; set; } 

        [Required(ErrorMessage = "Condition is required.")]
        [StringLength(50, ErrorMessage = "Condition cannot be longer than 50 characters.")]
        [Display(Name = "Condition")]
        public string Condition { get; set; }

        
        [Required(ErrorMessage = "Serial Number is required.")]
        [StringLength(50, ErrorMessage = "Serial Number cannot be longer than 50 characters.")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(100, ErrorMessage = "Supplier Name cannot be longer than 100 characters.")]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Supplier Contact is required.")]
        [StringLength(50, ErrorMessage = "Supplier Contact cannot be longer than 50 characters.")]
        [Display(Name = "Supplier Contact")]
        public string SupplierContact { get; set; }

        [Display(Name = "Delivery Documentation")]
        public byte[]? DeliveryDocumentation { get; set; }  // Nullable for optional documentation

        public string? DeliveryDocumentationFileName { get; set; }  // Nullable for optional file name

        [Display(Name = "Fridge Image")]
        public byte[]? FridgeImage { get; set; }  // Nullable for optional image

        public string? FridgeImageFileName { get; set; }  // Nullable for optional file name

        [Display(Name = "Warranty Start Date")]
        [DataType(DataType.Date)]
        public DateTime? WarrantyStartDate { get; set; }

        [Display(Name = "Warranty End Date")]
        [DataType(DataType.Date)]
        public DateTime? WarrantyEndDate { get; set; }

        [Display(Name = "Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters.")]
        public string? Note { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Is In Stock")]
        public bool IsInStock { get; set; } = true;

        [Display(Name = "Is Scrapped")]
        public bool IsScrapped { get; set; } = false;

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Is Allocated")]
        public bool IsAllocated { get; set; } = false;

        [Required(ErrorMessage = "Created By is required.")]
        [StringLength(100, ErrorMessage = "Created By cannot be longer than 100 characters.")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [StringLength(100, ErrorMessage = "Last Modified By cannot be longer than 100 characters.")]
        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }
        public int? EmployeeId { get; set; }
        [Display(Name = "Last Modified Date")]
        public DateTime? LastModifiedDate { get; set; }

        // Navigation property
        public ICollection<FridgeAllocation> FridgeAllocations { get; set; }
        public ICollection<ProcessAllocation> ProcessAllocations { get; set; }
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; }
        public ICollection<ScrappedFridge> ScrappedFridges { get; set; }
        public Employee Employee { get; set; }
    }
}
