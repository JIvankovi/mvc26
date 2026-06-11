using Microsoft.AspNetCore.Identity;

namespace projekt.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Category { get; set; } = "User";
    }
}