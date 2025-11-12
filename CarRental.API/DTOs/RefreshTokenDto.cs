using System.ComponentModel.DataAnnotations;

namespace CarRental.API.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
