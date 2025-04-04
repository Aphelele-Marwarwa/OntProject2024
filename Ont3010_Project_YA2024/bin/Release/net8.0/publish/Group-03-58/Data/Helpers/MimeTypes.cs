namespace Ont3010_Project_YA2024.Data.Helpers
{
    public static class MimeTypes
    {
        private static readonly Dictionary<string, string> _mimeTypes = new Dictionary<string, string>
        {
            { ".pdf", "application/pdf" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".xls", "application/vnd.ms-excel" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".doc", "application/msword" },
            // Add more file types as needed
        };

        public static string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return _mimeTypes.ContainsKey(extension) ? _mimeTypes[extension] : "application/octet-stream";
        }
    }
}
