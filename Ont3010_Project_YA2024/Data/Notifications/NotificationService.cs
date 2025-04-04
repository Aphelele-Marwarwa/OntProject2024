using Ont3010_Project_YA2024.Models;

namespace Ont3010_Project_YA2024.Data.Notifications
{
    public class NotificationService: INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
    }
}
