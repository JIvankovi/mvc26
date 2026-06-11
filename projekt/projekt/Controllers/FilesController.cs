using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.Models;

namespace projekt.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private const string UploadsFolderName = "uploads";

        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public FilesController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            var query = _dbContext.UploadedFiles.AsQueryable();
            if (!isAdmin && !string.IsNullOrWhiteSpace(currentUserId))
            {
                query = query.Where(file => file.UploadedByUserId == currentUserId);
            }

            var files = await query
                .OrderByDescending(file => file.UploadedAtUtc)
                .Select(file => new
                {
                    file.Id,
                    file.OriginalFileName,
                    file.StoredFileName,
                    file.RelativePath,
                    file.ContentType,
                    file.SizeBytes,
                    file.UploadedAtUtc
                })
                .ToListAsync();

            var result = files.Select(file => new
            {
                file.Id,
                file.OriginalFileName,
                file.StoredFileName,
                file.RelativePath,
                file.ContentType,
                file.SizeBytes,
                uploadedAtUtc = file.UploadedAtUtc.ToString("u"),
                downloadUrl = Url.Content($"~/{file.RelativePath}")
            });

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file was uploaded." });
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, UploadsFolderName);
            Directory.CreateDirectory(uploadsFolder);

            var originalFileName = Path.GetFileName(file.FileName);
            var fileExtension = Path.GetExtension(originalFileName);
            var safeFileName = $"{Guid.NewGuid():N}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            var relativePath = $"{UploadsFolderName}/{safeFileName}";

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            var uploadedFile = new UploadedFile
            {
                OriginalFileName = originalFileName,
                StoredFileName = safeFileName,
                RelativePath = relativePath,
                ContentType = file.ContentType,
                SizeBytes = file.Length,
                UploadedAtUtc = DateTime.UtcNow,
                UploadedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            _dbContext.UploadedFiles.Add(uploadedFile);
            await _dbContext.SaveChangesAsync();

            var fileUrl = Url.Content($"~/{relativePath}") ?? $"/{relativePath}";

            return Ok(new
            {
                uploadedFile.Id,
                fileName = originalFileName,
                storedName = safeFileName,
                relativePath,
                size = file.Length,
                contentType = file.ContentType,
                uploadedAtUtc = uploadedFile.UploadedAtUtc,
                url = fileUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var uploadedFile = await _dbContext.UploadedFiles.FirstOrDefaultAsync(file => file.Id == id);
            if (uploadedFile == null)
            {
                return NotFound(new { error = "File not found." });
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isOwner = string.Equals(uploadedFile.UploadedByUserId, currentUserId, StringComparison.Ordinal);
            var isAdmin = User.IsInRole("Admin");

            if (!isOwner && !isAdmin)
            {
                return Forbid();
            }

            var physicalPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, uploadedFile.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _dbContext.UploadedFiles.Remove(uploadedFile);
            await _dbContext.SaveChangesAsync();

            return Ok(new { id });
        }
    }
}