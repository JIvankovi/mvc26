using System.ComponentModel.DataAnnotations;

namespace projekt.Models
{
    public class UploadedFile
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(260)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(260)]
        public string StoredFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string RelativePath { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? ContentType { get; set; }

        public long SizeBytes { get; set; }

        public DateTime UploadedAtUtc { get; set; }

        [MaxLength(450)]
        public string? UploadedByUserId { get; set; }
    }
}