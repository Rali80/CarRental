using CarRental.MVC.Services;
using CarRental.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.MVC.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IApiService _apiService;

        public ReservationsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AccessToken") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var reservations = await _apiService.GetReservations();
            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int carId)
        {
            if (HttpContext.Session.GetString("AccessToken") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var car = await _apiService.GetCar(carId);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.Car = car;
            return View(new CreateReservationViewModel { CarId = carId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            if (HttpContext.Session.GetString("AccessToken") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var car = await _apiService.GetCar(model.CarId);
                ViewBag.Car = car;
                return View(model);
            }

            var result = await _apiService.CreateReservation(model);

            if (result != null)
            {
              
                TempData["Success"] = "Reservation created successfully";
                return RedirectToAction("Index");
            }

            
            ModelState.AddModelError("", "Error creating the reservation. Please verify that the dates are available.");
            var carData = await _apiService.GetCar(model.CarId);
            ViewBag.Car = carData;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            if (HttpContext.Session.GetString("AccessToken") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _apiService.CancelReservation(id);

            if (success)
            {
               
                TempData["Success"] = "Reservation cancelled successfully";
            }
            else
            {
                
                TempData["Error"] = "Error cancelling the reservation";
            }

            return RedirectToAction("Index");
        }
    }
}