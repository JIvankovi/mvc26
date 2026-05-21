using System;
using System.ComponentModel.DataAnnotations;

namespace projekt.ViewModels
{
    public class CalibrationFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public int DeviceId { get; set; }

        public string? DeviceName { get; set; }

        public int? TechnicianId { get; set; }

        public string? TechnicianName { get; set; }

        [Required]
        public DateTime CalibrationDateTime { get; set; }

        [StringLength(150)]
        public string? CalibrationStandard { get; set; }

        [Range(0, 1000)]
        public double MeasuredDeviation { get; set; }

        public bool PassedCalibration { get; set; }

        public DateTime? NextCalibrationDue { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
