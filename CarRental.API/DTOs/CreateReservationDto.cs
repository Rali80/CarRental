using System.ComponentModel.DataAnnotations;

namespace CarRental.API.DTOs
{
    public class CreateReservationDto
    {
        [Required]
        public int CarId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public string? Notes { get; set; }
    }
}
