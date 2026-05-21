using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projekt.Models
{
    public class Technician
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [MaxLength(150)]
        public string? Certification { get; set; }

        public int YearsOfExperience { get; set; }

        public virtual ICollection<Calibration> Calibrations { get; set; } = new List<Calibration>();
    }
}
