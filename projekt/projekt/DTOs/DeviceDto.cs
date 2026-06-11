using System;
using projekt.Models;

namespace projekt.DTOs
{
    public record DeviceDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Manufacturer { get; init; }
        public string? SerialNumber { get; init; }
        public DateTime? PurchaseDate { get; init; }
        public MeasurementType MeasurementType { get; init; }
    }
}
