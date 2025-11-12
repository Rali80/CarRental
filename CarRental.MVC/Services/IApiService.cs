using CarRental.MVC.ViewModels;

namespace CarRental.MVC.Services
{
    public interface IApiService
    {
        Task<AuthResponse?> Register(RegisterViewModel model);
        Task<AuthResponse?> Login(LoginViewModel model);
        Task<AuthResponse?> RefreshToken();
        Task Logout();
        Task<List<CarViewModel>> GetCars(bool? availableOnly = null);
        Task<CarViewModel?> GetCar(int id);
        Task<List<ReservationViewModel>> GetReservations();
        Task<ReservationViewModel?> GetReservation(int id);
        Task<ReservationViewModel?> CreateReservation(CreateReservationViewModel model);
        Task<bool> CancelReservation(int id);
        Task<CarViewModel?> CreateCar(CarViewModel model);
        Task<bool> UpdateCar(CarViewModel model);
        Task<bool> DeleteCar(int id);
        Task<List<UserViewModel>> GetAllUsers();
        Task<DashboardStatsViewModel?> GetDashboardStats();
        Task<List<ReservationViewModel>> GetRecentReservations(int limit = 10);
        Task<bool> UpdateReservationStatus(int id, string status);
    }
}
