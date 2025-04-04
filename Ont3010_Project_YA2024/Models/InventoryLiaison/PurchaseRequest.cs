using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ont3010_Project_YA2024.Models.InventoryLiaison
{
    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

   
    public class PurchaseRequest
    {
        [Key]
        public int PurchaseRequestId { get; set; }

        [Required(ErrorMessage = "Fridge Type is required.")]
        public int FridgeId { get; set; }  // Foreign key to Fridge

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Fridge Model Type is required.")]
        public string FridgeModelType { get; set; }


        public string? SerialNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [StringLength(50, ErrorMessage = "Capacity cannot exceed 50 characters.")]
        public string Capacity { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Urgency is required.")]
        [StringLength(20, ErrorMessage = "Urgency cannot exceed 20 characters.")]
        public string Urgency { get; set; }

        [Required(ErrorMessage = "Justification is required.")]
        [StringLength(500, ErrorMessage = "Justification cannot exceed 500 characters.")]
        public string Justification { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Required]
        [Display(Name = "Status")]
        [Column("RequestStatus")]  // Change the column name in the database
        public RequestStatus Status { get; set; }

        [Required]
        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; } = false;

        // Navigation property
        public virtual Fridge Fridge { get; set; }

        public int? SupplierId { get; set; }
        public Employee Employee { get; set; }
    }
}