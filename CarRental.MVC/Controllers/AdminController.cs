using CarRental.MVC.Services;
using CarRental.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IApiService _apiService;
        
        public AdminController(IApiService apiService)
        {
            _apiService = apiService;
        }
        
        private bool IsAdmin()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            return isAdmin == "True";
        }
        
        private IActionResult CheckAdminAccess()
        {
            if (HttpContext.Session.GetString("AccessToken") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            
            return null!;
        }
        
        public async Task<IActionResult> Index()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            var stats = await _apiService.GetDashboardStats();
            return View(stats);
        }
        
        public async Task<IActionResult> Cars()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            var cars = await _apiService.GetCars();
            return View(cars);
        }
        
        [HttpGet]
        public IActionResult CreateCar()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            return View(new CarViewModel());
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateCar(CarViewModel model)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var result = await _apiService.CreateCar(model);
            
            if (result != null)
            {
                TempData["Success"] = "Coche creado exitosamente";
                return RedirectToAction("Cars");
            }
            
            ModelState.AddModelError("", "Error al crear el coche");
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            var car = await _apiService.GetCar(id);
            
            if (car == null)
            {
                return NotFound();
            }
            
            return View(car);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditCar(CarViewModel model)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var success = await _apiService.UpdateCar(model);
            
            if (success)
            {
                TempData["Success"] = "Coche actualizado exitosamente";
                return RedirectToAction("Cars");
            }
            
            ModelState.AddModelError("", "Error al actualizar el coche");
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            var success = await _apiService.DeleteCar(id);
            
            if (success)
            {
                TempData["Success"] = "Coche eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el coche";
            }
            
            return RedirectToAction("Cars");
        }
        
        public async Task<IActionResult> Reservations()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            var reservations = await _apiService.GetReservations();
            return View(reservations);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateReservationStatus(int id, string status)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            var success = await _apiService.UpdateReservationStatus(id, status);
            
            if (success)
            {
                TempData["Success"] = "Estado actualizado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al actualizar el estado";
            }
            
            return RedirectToAction("Reservations");
        }
        
        public async Task<IActionResult> Users()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;
            
            var users = await _apiService.GetAllUsers();
            return View(users);
        }
    }
}
