// CarDto.cs (En tu proyecto CarRental.MVC)

namespace CarRental.MVC.DTOs // Asegúrate de usar el namespace correcto
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Passengers { get; set; }
    }
}