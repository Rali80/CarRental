using System.ComponentModel.DataAnnotations;

namespace CarRental.API.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        
        [Required]
        public int CarId { get; set; }
        
        public string? UserId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        
        public CarDto? Car { get; set; }
        public UserDto? User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
