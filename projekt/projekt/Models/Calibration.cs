using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projekt.Models
{
    public class Calibration
    {
        public int Id { get; set; }

        public DateTime CalibrationDateTime { get; set; }

        public int? DeviceId { get; set; }

        [ForeignKey(nameof(DeviceId))]
        public virtual Device? Device { get; set; }

        public int? TechnicianId { get; set; }

        [ForeignKey(nameof(TechnicianId))]
        public virtual Technician? Technician { get; set; }

        [MaxLength(150)]
        public string? CalibrationStandard { get; set; }

        public double MeasuredDeviation { get; set; }
        public bool PassedCalibration { get; set; }
        public DateTime? NextCalibrationDue { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
