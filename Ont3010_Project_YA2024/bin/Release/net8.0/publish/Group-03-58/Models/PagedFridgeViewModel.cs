using Ont3010_Project_YA2024.Models.InventoryLiaison;

namespace Ont3010_Project_YA2024.Models
{
    public class PagedFridgeViewModel
    {
        public List<Fridge> Fridges { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
