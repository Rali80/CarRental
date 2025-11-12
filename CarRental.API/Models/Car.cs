using System.ComponentModel.DataAnnotations;

namespace CarRental.API.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        public int Year { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string LicensePlate { get; set; } = string.Empty;
        
        [Required]
        public decimal PricePerDay { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        public bool IsAvailable { get; set; } = true;
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = "Sedan";
        
        public int Passengers { get; set; } = 5;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
