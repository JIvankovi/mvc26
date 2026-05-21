using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projekt.Models
{
    public class Device
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Manufacturer { get; set; }

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public MeasurementType MeasurementType { get; set; }

        public virtual ICollection<Calibration> CalibrationHistory { get; set; } = new List<Calibration>();
        public virtual ICollection<DeviceLocation> LocationHistory { get; set; } = new List<DeviceLocation>();
    }
}
