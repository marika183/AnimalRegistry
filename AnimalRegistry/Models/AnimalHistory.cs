using System.ComponentModel.DataAnnotations;

namespace AnimalRegistry.Models
{
    public class AnimalHistory
    {
        public int Id { get; set; }

        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        [Required]
        public string ChangeDescription { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
    }
}
