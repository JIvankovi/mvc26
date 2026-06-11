using System;

namespace projekt.DTOs
{
    public record CalibrationDto
    {
        public int Id { get; init; }
        public int DeviceId { get; init; }
        public string? DeviceName { get; init; }
        public int? TechnicianId { get; init; }
        public string? TechnicianName { get; init; }
        public DateTime CalibrationDateTime { get; init; }
        public string? CalibrationStandard { get; init; }
        public double MeasuredDeviation { get; init; }
        public bool PassedCalibration { get; init; }
        public DateTime? NextCalibrationDue { get; init; }
        public string? Notes { get; init; }
    }
}
