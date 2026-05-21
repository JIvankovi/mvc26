using System.ComponentModel.DataAnnotations;

namespace projekt.ViewModels
{
    public class TechnicianFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [StringLength(150)]
        public string? Certification { get; set; }

        [Range(0, 100)]
        public int YearsOfExperience { get; set; }
    }
}
