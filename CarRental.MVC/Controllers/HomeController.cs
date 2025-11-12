using CarRental.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;
        
        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async Task<IActionResult> Index()
        {
            var cars = await _apiService.GetCars(availableOnly: true);
            return View(cars);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var car = await _apiService.GetCar(id);
            
            if (car == null)
            {
                return NotFound();
            }
            
            return View(car);
        }
    }
}
