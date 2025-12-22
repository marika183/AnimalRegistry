using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AnimalRegistry.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Tutaj dodajemy nowe pola:

        [PersonalData] // Ta adnotacja oznacza, że są to dane wrażliwe użytkownika
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }

        [PersonalData]
        public string? Address { get; set; }

        // Relacja, którą już masz:
        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
