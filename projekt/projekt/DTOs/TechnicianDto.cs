namespace projekt.DTOs
{
    public record TechnicianDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Certification { get; init; }
        public int YearsOfExperience { get; init; }
    }
}
