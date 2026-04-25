using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPS.Shared.Abstractions;
using SPS.Shared.Options;
namespace SPS.Shared.Infrastructure.Services
{

    /// <summary>
    /// Service responsible for handling file uploads (images, documents, base64)
    /// Fully configurable via appsettings.json
    /// </summary>
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly UploadOptions _options;

        public FileService(IOptions<UploadOptions> options, ILogger<FileService> logger)
        {
            _options = options.Value;
            _logger = logger;
            EnsureBaseDirectoryExists();
        }

        private void EnsureBaseDirectoryExists()
        {
            if (!Directory.Exists(_options.BasePath))
            {
                Directory.CreateDirectory(_options.BasePath);
                _logger.LogInformation("Base upload directory created: {BasePath}", _options.BasePath);
            }
        }

        public async Task<string> SaveImageAsync(IFormFile formFile, string folderPath, string? fileName = null)
            => await SaveFileInternalAsync(formFile, folderPath, fileName, _options.AllowedImageExtensions, "image");

        public async Task<string> SaveFileAsync(IFormFile formFile, string folderPath, string? fileName = null)
            => await SaveFileInternalAsync(formFile, folderPath, fileName,
                _options.AllowedImageExtensions.Concat(_options.AllowedDocumentExtensions).ToArray(), "file");

        public async Task<string> SaveImageFromBase64Async(string base64String, string folderPath, string? fileName = null)
            => await SaveBase64InternalAsync(base64String, folderPath, fileName, _options.AllowedImageExtensions, "image");

        public async Task<string> SaveFileFromBase64Async(string base64String, string folderPath, string? fileName = null)
            => await SaveBase64InternalAsync(base64String, folderPath, fileName,
                _options.AllowedImageExtensions.Concat(_options.AllowedDocumentExtensions).ToArray(), "file");

        // ---------- Helper Methods ----------
        private async Task<string> SaveFileInternalAsync(IFormFile formFile, string folderPath, string? fileName,
            string[] allowedExtensions, string fileType)
        {
            if (formFile == null || formFile.Length == 0)
            {
                _logger.LogWarning("Empty {FileType} file uploaded.", fileType);
                return string.Empty;
            }

            var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid {FileType} extension: {Extension}", fileType, extension);
                return string.Empty;
            }

            if (!await EnsureDirectoryExistsAsync(folderPath)) return string.Empty;

            var finalFileName = GenerateFileName(fileName, extension);
            var fullPath = Path.Combine(_options.BasePath, folderPath.Trim('/'), finalFileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await formFile.CopyToAsync(stream);

            _logger.LogInformation("{FileType} saved: {FileName}", fileType, finalFileName);
            return finalFileName;
        }

        private async Task<string> SaveBase64InternalAsync(string base64String, string folderPath, string? fileName,
            string[] allowedExtensions, string fileType)
        {
            if (string.IsNullOrWhiteSpace(base64String)) return string.Empty;

            var (isSuccess, data, extension) = TryParseBase64(base64String);
            if (!isSuccess || !allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid Base64 or extension for {FileType}", fileType);
                return string.Empty;
            }

            if (!await EnsureDirectoryExistsAsync(folderPath)) return string.Empty;

            var finalFileName = GenerateFileName(fileName, extension);
            var fullPath = Path.Combine(_options.BasePath, folderPath.Trim('/'), finalFileName);

            await File.WriteAllBytesAsync(fullPath, data);
            _logger.LogInformation("{FileType} from Base64 saved: {FileName}", fileType, finalFileName);
            return finalFileName;
        }

        private async Task<bool> EnsureDirectoryExistsAsync(string folderPath)
        {
            var fullPath = Path.Combine(_options.BasePath, folderPath.Trim('/'));
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                await Task.CompletedTask;
            }
            return true;
        }

        private static string GenerateFileName(string? fileName, string extension)
            => string.IsNullOrWhiteSpace(fileName) ? $"{Guid.NewGuid()}{extension}" : $"{fileName.Trim()}{extension}";

        private static (bool success, byte[] data, string extension) TryParseBase64(string base64String)
        {
            var span = base64String.AsSpan().Trim();
            var dataStart = span.IndexOf("base64,");
            if (dataStart >= 0)
            {
                span = span[(dataStart + 7)..];
            }

            string extension = string.Empty;
            if (base64String.StartsWith("data:"))
            {
                var headerEnd = base64String.IndexOf(';');
                if (headerEnd > 5)
                {
                    var header = base64String[5..headerEnd];
                    extension = header switch
                    {
                        "image/jpeg" => ".jpg",
                        "image/png" => ".png",
                        "image/gif" => ".gif",
                        "application/pdf" => ".pdf",
                        _ => ""
                    };
                }
            }

            try
            {
                var bytes = Convert.FromBase64String(span.ToString());
                return (true, bytes, extension);
            }
            catch (FormatException)
            {
                return (false, Array.Empty<byte>(), string.Empty);
            }
        }
    }
}