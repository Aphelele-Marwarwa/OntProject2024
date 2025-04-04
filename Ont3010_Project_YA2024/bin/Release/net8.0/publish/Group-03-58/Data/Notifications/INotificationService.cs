using Ont3010_Project_YA2024.Models;

namespace Ont3010_Project_YA2024.Data.Notifications
{
    public interface INotificationService
    {
        Task AddNotificationAsync(Notification notification);
    }
}
