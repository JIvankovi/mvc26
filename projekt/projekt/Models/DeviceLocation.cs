using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projekt.Models
{
    public class DeviceLocation
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        [ForeignKey(nameof(DeviceId))]
        public virtual Device? Device { get; set; }

        public int? LaboratoryId { get; set; }

        [ForeignKey(nameof(LaboratoryId))]
        public virtual Laboratory? Laboratory { get; set; }

        public DateTime? AssignedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public bool IsCurrentLocation { get; set; }

        [MaxLength(300)]
        public string? AssignmentReason { get; set; }
    }
}
