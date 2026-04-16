namespace projekt.Models
{
    public class Technician
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Certification { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
    }
}
