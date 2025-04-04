namespace Ont3010_Project_YA2024.Data.Helpers
{
    public class EmailSettings
    {
        public string FromAddress { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public string DisplayName { get; set; }
    }
}
