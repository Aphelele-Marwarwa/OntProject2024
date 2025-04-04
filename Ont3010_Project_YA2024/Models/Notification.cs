using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false; // This applies to the creator of the notification
        public string ActionBy { get; set; } // The user who triggered the action (e.g., created employee)
        public DateTime Date { get; set; } = DateTime.Now;
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }

        // Track which employees have read this notification
        public ICollection<EmployeeNotificationStatus> EmployeeNotificationStatuses { get; set; }
    }
}
