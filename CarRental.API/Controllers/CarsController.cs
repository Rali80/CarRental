using CarRental.API.Data;
using CarRental.API.DTOs;
using CarRental.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetCars([FromQuery] bool? availableOnly = null)
        {
            var query = _context.Cars.AsQueryable();
            
            if (availableOnly == true)
            {
                query = query.Where(c => c.IsAvailable);
            }
            
            var cars = await query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CarDto
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    Color = c.Color,
                    LicensePlate = c.LicensePlate,
                    PricePerDay = c.PricePerDay,
                    ImageUrl = c.ImageUrl,
                    Description = c.Description,
                    IsAvailable = c.IsAvailable,
                    Category = c.Category,
                    Passengers = c.Passengers
                })
                .ToListAsync();
            
            return Ok(cars);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }
            
            return Ok(new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Color = car.Color,
                LicensePlate = car.LicensePlate,
                PricePerDay = car.PricePerDay,
                ImageUrl = car.ImageUrl,
                Description = car.Description,
                IsAvailable = car.IsAvailable,
                Category = car.Category,
                Passengers = car.Passengers
            });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CarDto>> CreateCar(CarDto dto)
        {
            var existingCar = await _context.Cars
                .FirstOrDefaultAsync(c => c.LicensePlate == dto.LicensePlate);
            
            if (existingCar != null)
            {
                return BadRequest(new { message = "License plate already exists" });
            }
            
            var car = new Car
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                Color = dto.Color,
                LicensePlate = dto.LicensePlate,
                PricePerDay = dto.PricePerDay,
                ImageUrl = dto.ImageUrl,
                Description = dto.Description,
                IsAvailable = dto.IsAvailable,
                Category = dto.Category,
                Passengers = dto.Passengers
            };
            
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            
            dto.Id = car.Id;
            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, dto);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, CarDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }
            
            var car = await _context.Cars.FindAsync(id);
            
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }
            
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Year = dto.Year;
            car.Color = dto.Color;
            car.LicensePlate = dto.LicensePlate;
            car.PricePerDay = dto.PricePerDay;
            car.ImageUrl = dto.ImageUrl;
            car.Description = dto.Description;
            car.IsAvailable = dto.IsAvailable;
            car.Category = dto.Category;
            car.Passengers = dto.Passengers;
            
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }
            
            var hasReservations = await _context.Reservations.AnyAsync(r => r.CarId == id);
            
            if (hasReservations)
            {
                return BadRequest(new { message = "Cannot delete car with existing reservations" });
            }
            
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
