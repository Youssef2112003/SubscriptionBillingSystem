using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPS.Application.Common;
using SPS.Infrastructure.Services.SPS.Infrastructure.Options;

namespace SPS.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IOptions<FileStorageOptions> options, ILogger<FileStorageService> logger)
        {
            _basePath = options.Value.BasePath;
            _logger = logger;
        }

        public async Task<string> SaveAsync(Stream stream, string fileName, string folderPath, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_basePath, folderPath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            using var fileStream = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fileStream, cancellationToken);
            return fullPath;
        }

        public async Task<string> SaveBase64Async(string base64String, string fileName, string folderPath, CancellationToken cancellationToken = default)
        {
            var bytes = Convert.FromBase64String(base64String);
            using var ms = new MemoryStream(bytes);
            return await SaveAsync(ms, fileName, folderPath, cancellationToken);
        }
    }

    // Options
    namespace SPS.Infrastructure.Options
    {
        public class FileStorageOptions
        {
            public const string SectionName = "FileStorage";
            public string BasePath { get; set; } = "Uploads";
        }
    }
}
