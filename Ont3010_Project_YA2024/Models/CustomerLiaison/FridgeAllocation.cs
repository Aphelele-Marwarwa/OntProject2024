using Microsoft.DotNet.Scaffolding.Shared.Project;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ont3010_Project_YA2024.Models.CustomerLiaison
{
    public class FridgeAllocation
    {
        [Key]
        public int FridgeAllocationId { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerId { get; set; }  // No need for ForeignKey attribute, convention handles this.

        [Required(ErrorMessage = "Fridge is required.")]
        public int FridgeId { get; set; }  // Convention handles this key relation.

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }


        [Display(Name = "Fridge Serial Number")]
        public string? SerialNumber { get; set; }

        [Required(ErrorMessage = "Allocation Date is required.")]
        [Display(Name = "Allocation Date")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(FridgeAllocation), "ValidateAllocationDate")]
        public DateTime AllocationDate { get; set; }

        [Required]
        [Range(1, 120, ErrorMessage = "Duration must be between 1 and 120 months.")]
        [Display(Name = "Duration (in months)")]
        public int Duration { get; set; }

        [StringLength(500, ErrorMessage = "Special Instructions cannot exceed 500 characters.")]
        [Display(Name = "Special Instructions")]
        public string SpecialInstructions { get; set; }

        public bool IsProcessed { get; set; } = false; // This is fine here, or you can initialize in the constructor.

        [Required]
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public Customer Customer { get; set; }

        public Fridge Fridge { get; set; }
        public Employee Employee { get; set; }

        public virtual ICollection<ProcessAllocation> ProcessAllocations { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public static ValidationResult ValidateAllocationDate(DateTime date, ValidationContext context)
        {
            if (date < DateTime.Today)
            {
                return new ValidationResult("Allocation Date cannot be in the past.");
            }
            return ValidationResult.Success;
        }
    }
}