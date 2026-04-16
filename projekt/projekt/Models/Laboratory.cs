using System.Collections.Generic;

namespace projekt.Models
{
    public class Laboratory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string BuildingCode { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public string ResponsiblePerson { get; set; } = string.Empty;
        public List<DeviceLocation> DeviceLocations { get; set; }

        public Laboratory()
        {
            DeviceLocations = new List<DeviceLocation>();
        }
    }
}
