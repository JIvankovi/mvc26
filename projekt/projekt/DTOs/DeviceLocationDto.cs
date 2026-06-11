using System;

namespace projekt.DTOs
{
    public record DeviceLocationDto
    {
        public int Id { get; init; }
        public int DeviceId { get; init; }
        public string? DeviceName { get; init; }
        public int? LaboratoryId { get; init; }
        public string? LaboratoryName { get; init; }
        public DateTime? AssignedDate { get; init; }
        public DateTime? RemovedDate { get; init; }
        public bool IsCurrentLocation { get; init; }
        public string? AssignmentReason { get; init; }
    }
}
