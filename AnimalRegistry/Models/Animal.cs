using System.ComponentModel.DataAnnotations;

namespace AnimalRegistry.Models
{
    public enum AnimalStatus
    {
        [Display(Name = "Oczekujący")]
        Oczekujacy = 0,
        [Display(Name = "Zatwierdzony")]
        Zatwierdzony = 1,
        [Display(Name = "Zarchiwizowany")]
        Zarchiwizowany = 2
    }

    public class Animal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Gatunek jest wymagany")]
        [Display(Name = "Gatunek zwierzęcia")]
        public string Species { get; set; }

        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        [DataType(DataType.Date)]
        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Display(Name = "Data zgonu")]
        [DataType(DataType.Date)]
        public DateTime? DateOfDeath { get; set; }

        [Required(ErrorMessage = "Numer identyfikacyjny jest wymagany")]
        [Display(Name = "Nr identyfikacyjny (Chip/Kolczyk)")]
        public string IdentificationNumber { get; set; }

        [Required(ErrorMessage = "Kraj pochodzenia jest wymagany")]
        [Display(Name = "Kraj pochodzenia")]
        public string CountryOfOrigin { get; set; }

        [Display(Name = "Źródło pochodzenia")]
        public string? Source { get; set; }

        [Display(Name = "Cel przetrzymywania")]
        public string? Purpose { get; set; }

        [Display(Name = "Dokumentacja PDF")]
        public string? VeterinaryCertificateUrl { get; set; }

        [Display(Name = "Status")]
        public AnimalStatus Status { get; set; } = AnimalStatus.Oczekujacy;

        public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

        public ICollection<AnimalHistory>? History { get; set; } = new List<AnimalHistory>();
    }
}