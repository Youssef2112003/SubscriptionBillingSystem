using Microsoft.AspNetCore.Http;

namespace SPS.Shared.Abstractions
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile formFile, string folderPath, string? fileName = null);
        Task<string> SaveFileAsync(IFormFile formFile, string folderPath, string? fileName = null);
        Task<string> SaveImageFromBase64Async(string base64String, string folderPath, string? fileName = null);
        Task<string> SaveFileFromBase64Async(string base64String, string folderPath, string? fileName = null);





    }
}
