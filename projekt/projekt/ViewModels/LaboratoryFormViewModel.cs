using System.ComponentModel.DataAnnotations;

namespace projekt.ViewModels
{
    public class LaboratoryFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(10)]
        public string? BuildingCode { get; set; }

        public int? RoomNumber { get; set; }

        [StringLength(200)]
        public string? ResponsiblePerson { get; set; }
    }
}
