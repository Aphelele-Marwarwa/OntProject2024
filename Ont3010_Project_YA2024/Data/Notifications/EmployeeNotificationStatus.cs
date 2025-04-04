    using Ont3010_Project_YA2024.Models.admin;
    using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.CustomerLiaison;

namespace Ont3010_Project_YA2024.Data.Notifications
{
    public class EmployeeNotificationStatus
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public Notification Notification { get; set; }

        public int? EmployeeId { get; set; } // Nullable for flexibility
        public Employee Employee { get; set; }

        public int? CustomerId { get; set; } // Nullable for flexibility
        public Customer Customer { get; set; }

        public bool IsRead { get; set; } = false; // Default to false for all users
    }
}
