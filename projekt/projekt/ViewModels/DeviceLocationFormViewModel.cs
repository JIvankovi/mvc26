using System;
using System.ComponentModel.DataAnnotations;

namespace projekt.ViewModels
{
    public class DeviceLocationFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public int DeviceId { get; set; }

        public string? DeviceName { get; set; }

        public int? LaboratoryId { get; set; }

        public string? LaboratoryName { get; set; }

        public DateTime? AssignedDate { get; set; }

        public DateTime? RemovedDate { get; set; }

        public bool IsCurrentLocation { get; set; }

        [StringLength(300)]
        public string? AssignmentReason { get; set; }
    }
}
