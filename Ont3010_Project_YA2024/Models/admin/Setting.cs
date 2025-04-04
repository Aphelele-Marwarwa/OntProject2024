using System.ComponentModel.DataAnnotations;

namespace Ont3010_Project_YA2024.Models.admin
{
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [Required(ErrorMessage = "Business Name is required.")]
        [StringLength(100, ErrorMessage = "Business Name cannot be longer than 100 characters.")]
        [Display(Name = "Business Name")]  
        public string BusinessName { get; set; }

       
        [Display(Name = "Business Logo")] 
        public byte[] BusinessLogo { get; set; }

      
        [Display(Name = "Cover Photo")] 
        public byte[] CoverPhoto { get; set; }

        [Required(ErrorMessage = "Contact Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Contact Email")]  
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Contact Phone is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [Display(Name = "Contact Phone")]  
        public string ContactPhone { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }  


        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }  

        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }  
    }
}
