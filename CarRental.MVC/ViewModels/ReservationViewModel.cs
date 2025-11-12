using System.ComponentModel.DataAnnotations;

namespace CarRental.MVC.ViewModels
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string? UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public CarViewModel? Car { get; set; }
        public UserViewModel? User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class CreateReservationViewModel
    {
        [Required(ErrorMessage = "Seleccione un coche")]
        public int CarId { get; set; }
        
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime EndDate { get; set; }
        
        public string? Notes { get; set; }
    }
    
    public class DashboardStatsViewModel
    {
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int TotalReservations { get; set; }
        public int PendingReservations { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
