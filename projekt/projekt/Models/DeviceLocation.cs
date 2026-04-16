using System;

namespace projekt.Models
{
    public class DeviceLocation
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; } = new Device();
        public int LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; } = new Laboratory();
        public DateTime AssignedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public bool IsCurrentLocation { get; set; }
        public string AssignmentReason { get; set; } = string.Empty;
    }
}
