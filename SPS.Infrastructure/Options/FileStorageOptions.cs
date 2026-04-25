namespace SPS.Infrastructure.Options
{
    public class FileStorageOptions
    {
        public const string SectionName = "UploadSettings";

        public string BasePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
        public string[] AllowedImageExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        public string[] AllowedDocumentExtensions { get; set; } = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };
    }
}
