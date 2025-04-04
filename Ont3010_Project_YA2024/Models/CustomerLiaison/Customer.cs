using Ont3010_Project_YA2024.Models.CustomerReport;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using System.ComponentModel.DataAnnotations;

namespace Ont3010_Project_YA2024.Models.CustomerLiaison
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }


        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(10, ErrorMessage = "Title cannot exceed 10 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
        [StringLength(100, ErrorMessage = "Email Address cannot exceed 100 characters.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number format.")]
        [StringLength(15, ErrorMessage = "Phone Number cannot exceed 15 characters.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Photo")]
        public byte[]? ProfilePhoto { get; set; }

        [StringLength(50, ErrorMessage = "Business Role cannot exceed 50 characters.")]
        [Display(Name = "Business Role")]
        public string BusinessRole { get; set; }

        [Required(ErrorMessage = "Business Name is required.")]
        [StringLength(100, ErrorMessage = "Business Name cannot exceed 100 characters.")]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Business Email Address format.")]
        [StringLength(100, ErrorMessage = "Business Email Address cannot exceed 100 characters.")]
        [Display(Name = "Business Email Address")]
        public string BusinessEmailAddress { get; set; }

        [Phone(ErrorMessage = "Invalid Business Phone Number format.")]
        [StringLength(15, ErrorMessage = "Business Phone Number cannot exceed 15 characters.")]
        [Display(Name = "Business Phone Number")]
        public string BusinessPhoneNumber { get; set; }

        [StringLength(50, ErrorMessage = "Business Type cannot exceed 50 characters.")]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }

        [StringLength(50, ErrorMessage = "Industry cannot exceed 50 characters.")]
        [Display(Name = "Industry")]
        public string Industry { get; set; }

        [Required(ErrorMessage = "Street Address is required.")]
        [StringLength(200, ErrorMessage = "Street Address cannot exceed 200 characters.")]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(10, ErrorMessage = "Postal Code cannot exceed 10 characters.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(50, ErrorMessage = "Province cannot exceed 50 characters.")]
        [Display(Name = "Province")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }


        [Required]
        [StringLength(256)]
        [Display(Name = "Password")]
        public string Password { get; set; }

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

        public bool HasAllocations => FridgeAllocations?.Any() == true || ProcessAllocations?.Any() == true;
        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; } = false;
       
        public ICollection<FridgeAllocation> FridgeAllocations { get; set; }
        public ICollection<ProcessAllocation> ProcessAllocations { get; set; }
       
    }
}