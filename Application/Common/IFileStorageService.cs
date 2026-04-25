namespace SPS.Application.Common;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream stream, string fileName, string folderPath, CancellationToken cancellationToken = default);
    Task<string> SaveBase64Async(string base64String, string fileName, string folderPath, CancellationToken cancellationToken = default);
}