using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
namespace Ont3010_Project_YA2024.Models.CustomerReport
{
    public class NewFridgeRequest
    {

        [Key]
        public int NewFridgeRequestId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }  // Navigation property

        [Required]
        [Display(Name = "Application Date")]
        [DataType(DataType.Date)]
        public DateTime DateApplied { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100, ErrorMessage = "Fridge Model/Type cannot be longer than 100 characters.")]
        [Display(Name = "Fridge Model/Type")]
        public string ModelType { get; set; }

        [Required(ErrorMessage = "Fridge Capacity is required")]
        [Display(Name = "Fridge Capacity")]
        public string Capacity { get; set; }

        [Required(ErrorMessage ="Condition is required")]
        [Display(Name ="Condition")]
        public string Condition { get; set; }

        [Required(ErrorMessage ="Duration is required")]
        [Display(Name ="Duration")]
        public string Duration { get; set; }

        public string Status { get; set; }


    }
}
