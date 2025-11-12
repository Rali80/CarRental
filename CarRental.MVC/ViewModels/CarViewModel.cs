using System.ComponentModel.DataAnnotations;

namespace CarRental.MVC.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "La marca es requerida")]
        public string Brand { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El modelo es requerido")]
        public string Model { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El año es requerido")]
        [Range(1900, 2100, ErrorMessage = "Año inválido")]
        public int Year { get; set; }
        
        [Required(ErrorMessage = "El color es requerido")]
        public string Color { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La placa es requerida")]
        public string LicensePlate { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 10000, ErrorMessage = "El precio debe estar entre 0.01 y 10000")]
        public decimal PricePerDay { get; set; }
        
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        [Required(ErrorMessage = "La categoría es requerida")]
        public string Category { get; set; } = "Sedan";
        
        [Range(1, 20, ErrorMessage = "Los pasajeros deben estar entre 1 y 20")]
        public int Passengers { get; set; } = 5;
    }
}
