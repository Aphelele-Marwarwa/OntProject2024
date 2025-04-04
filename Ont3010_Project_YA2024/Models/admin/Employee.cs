using Microsoft.AspNetCore.Identity;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using Ont3010_Project_YA2024.Models.InventoryLiaison;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ont3010_Project_YA2024.Models.admin
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
        [StringLength(100, ErrorMessage = "Email Address cannot exceed 100 characters.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\+27|0)(\d{9})$", ErrorMessage = "Phone number must be in South African format")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Hire")]
        public DateTime DateOfHire { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Responsibility")]
        public string Responsibility { get; set; }

        [Required]
        [StringLength(256)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Display(Name = "Profile Photo")]
        public byte[]? ProfilePhoto { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;

        public ICollection<FridgeAllocation> FridgeAllocations { get; set; }
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; }
        public ICollection<ScrappedFridge> ScrappedFridges { get; set; }

    }
}
