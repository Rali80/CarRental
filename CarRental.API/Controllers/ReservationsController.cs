using CarRental.API.Data;
using CarRental.API.DTOs;
using CarRental.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            var query = _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .AsQueryable();
            
            if (!isAdmin)
            {
                query = query.Where(r => r.UserId == userId);
            }
            
            var reservations = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    CarId = r.CarId,
                    UserId = r.UserId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status,
                    Notes = r.Notes,
                    CreatedAt = r.CreatedAt,
                    Car = new CarDto
                    {
                        Id = r.Car.Id,
                        Brand = r.Car.Brand,
                        Model = r.Car.Model,
                        Year = r.Car.Year,
                        Color = r.Car.Color,
                        LicensePlate = r.Car.LicensePlate,
                        PricePerDay = r.Car.PricePerDay,
                        ImageUrl = r.Car.ImageUrl,
                        Category = r.Car.Category
                    },
                    User = new UserDto
                    {
                        Id = r.User.Id,
                        Email = r.User.Email ?? string.Empty,
                        FirstName = r.User.FirstName,
                        LastName = r.User.LastName
                    }
                })
                .ToListAsync();
            
            return Ok(reservations);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found" });
            }
            
            if (!isAdmin && reservation.UserId != userId)
            {
                return Forbid();
            }
            
            return Ok(new ReservationDto
            {
                Id = reservation.Id,
                CarId = reservation.CarId,
                UserId = reservation.UserId,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                TotalPrice = reservation.TotalPrice,
                Status = reservation.Status,
                Notes = reservation.Notes,
                CreatedAt = reservation.CreatedAt,
                Car = new CarDto
                {
                    Id = reservation.Car.Id,
                    Brand = reservation.Car.Brand,
                    Model = reservation.Car.Model,
                    Year = reservation.Car.Year,
                    PricePerDay = reservation.Car.PricePerDay,
                    ImageUrl = reservation.Car.ImageUrl
                },
                User = new UserDto
                {
                    Id = reservation.User.Id,
                    Email = reservation.User.Email ?? string.Empty,
                    FirstName = reservation.User.FirstName,
                    LastName = reservation.User.LastName
                }
            });
        }
        
        [HttpPost]
        public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            if (dto.StartDate < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Start date cannot be in the past" });
            }
            
            if (dto.EndDate <= dto.StartDate)
            {
                return BadRequest(new { message = "End date must be after start date" });
            }
            
            var car = await _context.Cars.FindAsync(dto.CarId);
            
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }
            
            if (!car.IsAvailable)
            {
                return BadRequest(new { message = "Car is not available" });
            }
            
            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.CarId == dto.CarId &&
                              r.Status != "Cancelled" &&
                              ((dto.StartDate >= r.StartDate && dto.StartDate < r.EndDate) ||
                               (dto.EndDate > r.StartDate && dto.EndDate <= r.EndDate) ||
                               (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate)));
            
            if (hasConflict)
            {
                return BadRequest(new { message = "Car is already reserved for the selected dates" });
            }
            
            var days = (dto.EndDate - dto.StartDate).Days;
            var totalPrice = days * car.PricePerDay;
            
            var reservation = new Reservation
            {
                CarId = dto.CarId,
                UserId = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = totalPrice,
                Status = "Pending",
                Notes = dto.Notes
            };
            
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            
            var result = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstAsync(r => r.Id == reservation.Id);
            
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, new ReservationDto
            {
                Id = result.Id,
                CarId = result.CarId,
                UserId = result.UserId,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                TotalPrice = result.TotalPrice,
                Status = result.Status,
                Notes = result.Notes,
                CreatedAt = result.CreatedAt
            });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateReservationStatus(int id, [FromBody] string status)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found" });
            }
            
            var validStatuses = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };
            
            if (!validStatuses.Contains(status))
            {
                return BadRequest(new { message = "Invalid status" });
            }
            
            reservation.Status = status;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found" });
            }
            
            if (!isAdmin && reservation.UserId != userId)
            {
                return Forbid();
            }
            
            if (reservation.Status == "Completed")
            {
                return BadRequest(new { message = "Cannot cancel completed reservation" });
            }
            
            reservation.Status = "Cancelled";
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
