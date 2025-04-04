using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ont3010_Project_YA2024.Models.InventoryLiaison
{
   
    public class ProcessAllocation
    {
        [Key]
        public int ProcessAllocationId { get; set; }

        [Required]
        [Display(Name = "Fridge Allocation ID")]
        public int FridgeAllocationId { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(100, ErrorMessage = "Customer Name cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Customer Last")]
        [StringLength(100, ErrorMessage = "Customer Name cannot exceed 100 characters.")]
        public string CustomerLast { get; set; }

        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Fridge ID")]
        [ForeignKey(nameof(Fridge))]
        public int FridgeId { get; set; }

        [Required]
        [Display(Name = "Fridge Serial Number")]
        public string SerialNumber { get; set; }

        [Required]
        [Display(Name = "Allocation Date")]
        public DateTime AllocationDate { get; set; }

        [Required]
        [Display(Name = "Delivery/Pickup Date")]
        public DateTime DeliveryPickupDate { get; set; }

        [Display(Name = "Special Instructions")]
        [StringLength(500, ErrorMessage = "Special Instructions cannot exceed 500 characters.")]
        public string? SpecialInstructions { get; set; }

        [Required]
        [Display(Name = "Approval Status")]
        [StringLength(20, ErrorMessage = "Approval Status cannot exceed 20 characters.")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "Approval Note")]
        [StringLength(500, ErrorMessage = "Approval Note cannot exceed 500 characters.")]
        public string? ApprovalNote { get; set; }

        [Display(Name = "Last Modified By")]
        [StringLength(100, ErrorMessage = "Last Modified By cannot exceed 100 characters.")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified Date")]
        public DateTime? LastModifiedDate { get; set; }

        // Navigation properties
        public virtual FridgeAllocation FridgeAllocation { get; set; }
        public virtual Fridge Fridge { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
    }
}