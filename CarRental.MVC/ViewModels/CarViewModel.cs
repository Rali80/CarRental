using System.ComponentModel.DataAnnotations;

namespace CarRental.MVC.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "License plate is required")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per day is required")]
        [Range(0.01, 10000, ErrorMessage = "Price per day must be between 0.01 and 10,000")]
        public decimal PricePerDay { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = "Sedan";

        [Range(1, 20, ErrorMessage = "Passenger count must be between 1 and 20")]
        public int Passengers { get; set; } = 5;
    }
}
