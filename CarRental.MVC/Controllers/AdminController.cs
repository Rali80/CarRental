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
                // Access Check: Redirect to Login if not authenticated
                return RedirectToAction("Login", "Account");
            }

            if (!IsAdmin())
            {
                // Access Check: Redirect to Home if not an Admin
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
        public async Task<IActionResult> CreateCar(CarViewModel carViewModel)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (!ModelState.IsValid)
            {
                return View(carViewModel);
            }

            var result = await _apiService.CreateCar(carViewModel);

            if (result != null)
            {
                // MENSAJE CAMBIADO: "Coche creado exitosamente"
                TempData["Success"] = "Car created successfully";
                return RedirectToAction("Cars");
            }

            // MENSAJE CAMBIADO: "Error al crear el coche"
            ModelState.AddModelError("", "Error creating the car");
            return View(carViewModel);
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
        public async Task<IActionResult> EditCar(CarViewModel carViewModel)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (!ModelState.IsValid)
            {
                return View(carViewModel);
            }

            var success = await _apiService.UpdateCar(carViewModel);

            if (success)
            {
                // MENSAJE CAMBIADO: "Coche actualizado exitosamente"
                TempData["Success"] = "Car updated successfully";
                return RedirectToAction("Cars");
            }

            // MENSAJE CAMBIADO: "Error al actualizar el coche"
            ModelState.AddModelError("", "Error updating the car");
            return View(carViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var success = await _apiService.DeleteCar(id);

            if (success)
            {
                // MENSAJE CAMBIADO: "Coche eliminado exitosamente"
                TempData["Success"] = "Car deleted successfully";
            }
            else
            {
                // MENSAJE CAMBIADO: "Error al eliminar el coche"
                TempData["Error"] = "Error deleting the car";
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
                // MENSAJE CAMBIADO: "Estado actualizado exitosamente"
                TempData["Success"] = "Status updated successfully";
            }
            else
            {
                // MENSAJE CAMBIADO: "Error al actualizar el estado"
                TempData["Error"] = "Error updating the status";
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