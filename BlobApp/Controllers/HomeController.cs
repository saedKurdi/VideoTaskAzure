using BlobApp.Models;
using BlobApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlobApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlobStorageService blobStorageService;

        public HomeController(IBlobStorageService blobStorageService)
        {
            this.blobStorageService = blobStorageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".mp4", ".mov", ".avi" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type.");
            }

            var contentType = file.ContentType; // automatically determines content type
            using var stream = file.OpenReadStream();
            var fileName = Guid.NewGuid().ToString() + fileExtension;

            var fileUrl = await blobStorageService.UploadAsync(stream, fileName);
            return RedirectToAction("Index");
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var stream = await blobStorageService.DownloadAsync(fileName);

            var fileExtension = Path.GetExtension(fileName).ToLower();
            var contentType = fileExtension switch
            {
                ".jpg" or ".jpeg" or ".png" => "image/jpeg",
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                _ => "application/octet-stream"
            };

            return File(stream, contentType, fileName);
        }

        public async  Task<IActionResult> Index()
        {
            var imageUrls = await blobStorageService.ListFilesAsync();
            return View(imageUrls);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
