using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.API.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CarId { get; set; }
        
        [ForeignKey("CarId")]
        public Car Car { get; set; } = null!;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
