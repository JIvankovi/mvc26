using System.Collections.Generic;

namespace projekt.Models
{
    public class Laboratory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string BuildingCode { get; set; }
        public int RoomNumber { get; set; }
        public string ResponsiblePerson { get; set; }
        public List<DeviceLocation> DeviceLocations { get; set; }

        public Laboratory()
        {
            DeviceLocations = new List<DeviceLocation>();
        }
    }
}
