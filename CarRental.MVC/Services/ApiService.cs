using CarRental.MVC.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CarRental.MVC.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        
        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
        }
        
        private void SetAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        
        public async Task<AuthResponse?> Register(RegisterViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Auth/register", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    SaveAuthData(result);
                }
                return result;
            }
            
            return null;
        }
        
        public async Task<AuthResponse?> Login(LoginViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Auth/login", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    SaveAuthData(result);
                }
                return result;
            }
            
            return null;
        }
        
        public async Task<AuthResponse?> RefreshToken()
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Session.GetString("RefreshToken");
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }
            
            var content = new StringContent(JsonSerializer.Serialize(new { refreshToken }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Auth/refresh", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    SaveAuthData(result);
                }
                return result;
            }
            
            return null;
        }
        
        public Task Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
            return Task.CompletedTask;
        }
        
        private void SaveAuthData(AuthResponse authResponse)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString("AccessToken", authResponse.AccessToken);
                session.SetString("RefreshToken", authResponse.RefreshToken);
                session.SetString("UserId", authResponse.User.Id);
                session.SetString("UserEmail", authResponse.User.Email);
                session.SetString("UserFirstName", authResponse.User.FirstName);
                session.SetString("UserLastName", authResponse.User.LastName);
                session.SetString("IsAdmin", authResponse.User.IsAdmin.ToString());
            }
        }
        
        public async Task<List<CarViewModel>> GetCars(bool? availableOnly = null)
        {
            var url = "/api/Cars";
            if (availableOnly.HasValue)
            {
                url += $"?availableOnly={availableOnly.Value}";
            }
            
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CarViewModel>>() ?? new List<CarViewModel>();
            }
            
            return new List<CarViewModel>();
        }
        
        public async Task<CarViewModel?> GetCar(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Cars/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CarViewModel>();
            }
            
            return null;
        }
        
        public async Task<List<ReservationViewModel>> GetReservations()
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync("/api/Reservations");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ReservationViewModel>>() ?? new List<ReservationViewModel>();
            }
            
            return new List<ReservationViewModel>();
        }
        
        public async Task<ReservationViewModel?> GetReservation(int id)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"/api/Reservations/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ReservationViewModel>();
            }
            
            return null;
        }
        
        public async Task<ReservationViewModel?> CreateReservation(CreateReservationViewModel model)
        {
            SetAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Reservations", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ReservationViewModel>();
            }
            
            return null;
        }
        
        public async Task<bool> CancelReservation(int id)
        {
            SetAuthHeader();
            var response = await _httpClient.DeleteAsync($"/api/Reservations/{id}");
            return response.IsSuccessStatusCode;
        }
        
        public async Task<CarViewModel?> CreateCar(CarViewModel model)
        {
            SetAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Cars", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CarViewModel>();
            }
            
            return null;
        }
        
        public async Task<bool> UpdateCar(CarViewModel model)
        {
            SetAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Cars/{model.Id}", content);
            return response.IsSuccessStatusCode;
        }
        
        public async Task<bool> DeleteCar(int id)
        {
            SetAuthHeader();
            var response = await _httpClient.DeleteAsync($"/api/Cars/{id}");
            return response.IsSuccessStatusCode;
        }
        
        public async Task<List<UserViewModel>> GetAllUsers()
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync("/api/Admin/users");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserViewModel>>() ?? new List<UserViewModel>();
            }
            
            return new List<UserViewModel>();
        }
        
        public async Task<DashboardStatsViewModel?> GetDashboardStats()
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync("/api/Admin/dashboard");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DashboardStatsViewModel>();
            }
            
            return null;
        }
        
        public async Task<List<ReservationViewModel>> GetRecentReservations(int limit = 10)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"/api/Admin/reservations/recent?limit={limit}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ReservationViewModel>>() ?? new List<ReservationViewModel>();
            }
            
            return new List<ReservationViewModel>();
        }
        
        public async Task<bool> UpdateReservationStatus(int id, string status)
        {
            SetAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(status), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Reservations/{id}/status", content);
            return response.IsSuccessStatusCode;
        }
    }
}
