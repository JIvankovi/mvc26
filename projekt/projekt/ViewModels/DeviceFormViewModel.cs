using System;
using System.ComponentModel.DataAnnotations;
using projekt.Models;

namespace projekt.ViewModels
{
    public class DeviceFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Manufacturer { get; set; }

        [StringLength(100)]
        public string? SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }

        [Required]
        public MeasurementType MeasurementType { get; set; }
    }
}
