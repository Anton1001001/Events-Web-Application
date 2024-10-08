using EventsWebApplication.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EventsWebApplication.Infrastructure.FileStorage;
public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folderPath)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
        
        var filePath = Path.Combine(uploadPath, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        return filePath;
    }
}