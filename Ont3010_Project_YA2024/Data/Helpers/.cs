namespace Ont3010_Project_YA2024.Data.Helpers
{
    public interface IEmailSender
    {
          Task SendEmailAsync(string email, string subject, string message);
    }
}
