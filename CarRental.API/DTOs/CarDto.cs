using System.ComponentModel.DataAnnotations;

namespace CarRental.API.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Brand { get; set; } = string.Empty;
        
        [Required]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }
        
        [Required]
        public string Color { get; set; } = string.Empty;
        
        [Required]
        public string LicensePlate { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, 10000)]
        public decimal PricePerDay { get; set; }
        
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; } = "Sedan";
        
        [Range(1, 20)]
        public int Passengers { get; set; } = 5;
    }
}
