using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projekt.Data;
using projekt.Diagnostics;
using projekt.Models;

namespace projekt.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private const string UploadsFolderName = "uploads";
        private const string UploadsStorageFolderName = "projekt-uploads";

        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FilesController> _logger;

        public FilesController(ApplicationDbContext dbContext, IWebHostEnvironment environment, ILogger<FilesController> logger)
        {
            _dbContext = dbContext;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var uploadedFile = await _dbContext.UploadedFiles.FirstOrDefaultAsync(file => file.Id == id);
            if (uploadedFile == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isOwner = string.Equals(uploadedFile.UploadedByUserId, currentUserId, StringComparison.Ordinal);
            var isAdmin = User.IsInRole("Admin");
            if (!isOwner && !isAdmin)
            {
                return Forbid();
            }

            var physicalPath = GetPhysicalPath(uploadedFile.RelativePath);
            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound();
            }

            var contentType = string.IsNullOrWhiteSpace(uploadedFile.ContentType)
                ? "application/octet-stream"
                : uploadedFile.ContentType;

            return PhysicalFile(physicalPath, contentType, uploadedFile.OriginalFileName);
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
                relativePath = file.RelativePath ?? string.Empty,
                diskPath = GetPhysicalPath(file.RelativePath),
                file.ContentType,
                file.SizeBytes,
                uploadedAtUtc = file.UploadedAtUtc.ToString("u"),
                downloadUrl = Url.Action(nameof(Download), "Files", new { id = file.Id })
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
                _logger.LogWarning("Upload rejected because no file payload was provided.");
                return BadRequest(new { error = "No file was uploaded." });
            }

            var originalFileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                originalFileName = "uploaded-file";
            }

            var currentUserId = TrimToLength(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, 450);
            _logger.LogInformation(
                "Uploading file {FileName} ({SizeBytes} bytes) for user {UserId}.",
                originalFileName,
                file.Length,
                currentUserId ?? "anonymous");

            var uploadsFolder = GetStorageRoot();
            Directory.CreateDirectory(uploadsFolder);

            var fileExtension = Path.GetExtension(originalFileName);
            var safeFileName = $"{Guid.NewGuid():N}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            var relativePath = $"{UploadsFolderName}/{safeFileName}";

            try
            {
                await using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);

                var uploadedFile = new UploadedFile
                {
                    OriginalFileName = TrimToLength(originalFileName, 260) ?? "uploaded-file",
                    StoredFileName = safeFileName,
                    RelativePath = relativePath,
                    ContentType = TrimToLength(file.ContentType, 256),
                    SizeBytes = file.Length,
                    UploadedAtUtc = DateTime.UtcNow,
                    UploadedByUserId = currentUserId
                };

                _dbContext.UploadedFiles.Add(uploadedFile);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Upload completed for file {FileName} with database id {FileId}.",
                    originalFileName,
                    uploadedFile.Id);

                return Ok(new
                {
                    uploadedFile.Id,
                    fileName = originalFileName,
                    storedName = safeFileName,
                    relativePath,
                    size = file.Length,
                    contentType = file.ContentType,
                    uploadedAtUtc = uploadedFile.UploadedAtUtc,
                    url = Url.Action(nameof(Download), "Files", new { id = uploadedFile.Id })
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Upload failed for file {FileName} ({SizeBytes} bytes) for user {UserId}.",
                    originalFileName,
                    file.Length,
                    currentUserId ?? "anonymous");

                RuntimeFileLog.WriteException(
                    _environment.ContentRootPath,
                    $"Upload failed for file '{originalFileName}' ({file.Length} bytes) for user '{currentUserId ?? "anonymous"}'.",
                    exception);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return Problem(
                    title: "Upload failed",
                    detail: exception.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
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

            var physicalPath = GetPhysicalPath(uploadedFile.RelativePath);
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _dbContext.UploadedFiles.Remove(uploadedFile);
            await _dbContext.SaveChangesAsync();

            return Ok(new { id });
        }

        private string GetPhysicalPath(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }

            var root = GetStorageRoot();
            var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
            var trimmed = normalized.StartsWith($"{UploadsFolderName}{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                ? normalized[(UploadsFolderName.Length + 1)..]
                : normalized;

            return Path.Combine(root, trimmed);
        }

        private string GetStorageRoot()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, UploadsStorageFolderName);
        }

        private static string? TrimToLength(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value[..maxLength];
        }
    }
}