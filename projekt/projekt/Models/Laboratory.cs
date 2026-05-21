using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projekt.Models
{
    public class Laboratory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(10)]
        public string? BuildingCode { get; set; }
        public int? RoomNumber { get; set; }

        [MaxLength(200)]
        public string? ResponsiblePerson { get; set; }

        public virtual ICollection<DeviceLocation> DeviceLocations { get; set; } = new List<DeviceLocation>();
    }
}
