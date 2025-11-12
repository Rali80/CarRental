using CarRental.API.Data;
using CarRental.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsAdmin = u.IsAdmin
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            var totalCars = await _context.Cars.CountAsync();
            var availableCars = await _context.Cars.CountAsync(c => c.IsAvailable);
            var totalReservations = await _context.Reservations.CountAsync();
            var pendingReservations = await _context.Reservations.CountAsync(r => r.Status == "Pending");
            var totalUsers = await _context.Users.CountAsync();
            var totalRevenue = await _context.Reservations
                .Where(r => r.Status == "Completed")
                .SumAsync(r => (decimal?)r.TotalPrice) ?? 0;

            return Ok(new
            {
                totalCars,
                availableCars,
                totalReservations,
                pendingReservations,
                totalUsers,
                totalRevenue
            });
        }

        [HttpGet("reservations/recent")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetRecentReservations([FromQuery] int limit = 10)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(limit)
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
                        LicensePlate = r.Car.LicensePlate
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
    }
}
