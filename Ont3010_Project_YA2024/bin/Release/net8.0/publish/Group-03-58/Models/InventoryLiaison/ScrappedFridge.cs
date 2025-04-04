using Ont3010_Project_YA2024.Models.admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ont3010_Project_YA2024.Models.InventoryLiaison
{
    public class ScrappedFridge
    {

        [Key]
        public int ScrappedFridgeId { get; set; }

        [Required(ErrorMessage = "Fridge ID is required.")]
        [Display(Name = "Fridge ID")]
        [ForeignKey(nameof(Fridge))]
        public int FridgeId { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Fridge Serial Number")]
        public string? FridgeSerialNumber { get; set; }  // Nullable string


        [Required(ErrorMessage = "Scrap Date is required.")]
        [Display(Name = "Scrap Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime ScrapDate { get; set; }

        [Required(ErrorMessage = "Reason for Scrapping is required.")]
        [StringLength(500, ErrorMessage = "Scrap Reason cannot be longer than 500 characters.")]
        [Display(Name = "Reason for Scrapping")]
        public string ScrapReason { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot be longer than 1000 characters.")]
        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }  // Consolidated property

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        public Employee Employee { get; set; }
        // Navigation property
        public virtual Fridge Fridge { get; set; }
    }
}