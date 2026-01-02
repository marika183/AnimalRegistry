using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AnimalRegistry.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData] 
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }

        [PersonalData]
        public string? Address { get; set; }

        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
