using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Ont3010_Project_YA2024.Models.admin
{
    public class Business
    {
        [Key]
        public int BusinessId { get; set; }

        [Required(ErrorMessage = "Location Name is required.")]
        [StringLength(100, ErrorMessage = "Location Name cannot be longer than 100 characters.")]
        [Display(Name = "Location Name")]
        public string LocationName { get; set; }

        [Required(ErrorMessage = "Slogan is required.")]
        [StringLength(100, ErrorMessage = "Slogan cannot be longer than 100 characters.")]
        [Display(Name = "Slogan")]
        public string Slogan { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        [StringLength(200, ErrorMessage = "Street cannot be longer than 200 characters.")]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot be longer than 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(10, ErrorMessage = "Postal Code cannot exceed 10 characters.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "State/Province is required.")]
        [StringLength(100, ErrorMessage = "State/Province cannot be longer than 100 characters.")]
        [Display(Name = "Province")]
        public string StateProvince { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country cannot be longer than 100 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Contact Person is required.")]
        [StringLength(100, ErrorMessage = "Contact Person cannot be longer than 100 characters.")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Contact Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

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

        public ICollection<Setting> Settings { get; set; }  

        public Business()
        {
            Settings = new Collection<Setting>(); 
        }
    }
}
