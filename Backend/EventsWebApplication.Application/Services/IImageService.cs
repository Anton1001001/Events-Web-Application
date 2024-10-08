using Microsoft.AspNetCore.Http;

namespace EventsWebApplication.Application.Services;

public interface IImageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderPath);
}