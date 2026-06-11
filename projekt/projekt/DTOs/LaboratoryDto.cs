using System;

namespace projekt.DTOs
{
    public record LaboratoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Location { get; init; }
        public string? BuildingCode { get; init; }
        public int? RoomNumber { get; init; }
        public string? ResponsiblePerson { get; init; }
    }
}
