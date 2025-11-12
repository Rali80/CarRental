using CarRental.API.Data;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already registered" });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Registration failed", errors = result.Errors });
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshToken(user.Id);

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsAdmin = user.IsAdmin
                }
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshToken(user.Id);

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsAdmin = user.IsAdmin
                }
            });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenDto dto)
        {
            var refreshToken = await _tokenService.ValidateRefreshToken(dto.RefreshToken);

            if (refreshToken == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            refreshToken.IsUsed = true;
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = await _tokenService.CreateRefreshToken(user.Id);

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsAdmin = user.IsAdmin
                }
            });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken(RefreshTokenDto dto)
        {
            await _tokenService.RevokeRefreshToken(dto.RefreshToken);
            return Ok(new { message = "Token revoked successfully" });
        }
    }
}

