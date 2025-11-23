
CarRental - Full-Stack Car Rental System (.NET 9 + MVC + JWT)
.NET 9
License: MIT

Overview
CarRental is a modern, full-stack car rental platform built with .NET 9 that includes:

A clean RESTful API (ASP.NET Core Web API)
A responsive MVC frontend (Razor Pages + Bootstrap)
Secure authentication using JWT + Refresh Token flow
Role-based authorization (Admin and User roles)
Complete admin dashboard with real-time statistics
Car management, reservation system with conflict detection, and revenue tracking
Perfect for learning modern .NET architecture, JWT authentication, or as a production-ready base for a real car rental business.

Features
Users & Authentication
Register / Login with email & password
JWT access tokens (2-hour expiry)
Secure refresh token system (7-day expiry, revocable)
Automatic logout with token revocation
Session-based client state (MVC)
Customer Features
Browse all available cars
View car details (brand, model, price, images, etc.)
Make reservations with date validation
View and cancel own reservations
Admin Features (protected routes)
Full CRUD for cars
Dashboard with key metrics:
Total cars & available cars
Total reservations & pending ones
Registered users count
Total revenue from completed rentals
View all users
View recent reservations with customer & car details
Update reservation status (Pending → Confirmed → Completed)
Technical Highlights
Clean separation: API + MVC client
Entity Framework Core + SQL Server (LocalDB)
Proper DTOs and ViewModels
Refresh token revocation & cleanup
CORS properly configured
Swagger UI included for API testing
Full English internationalization (UI + validation messages)
Tech Stack
Layer	Technology
Backend	ASP.NET Core 9.0 Web API
Frontend	ASP.NET Core MVC + Razor + Bootstrap
Authentication	JWT Bearer + Refresh Tokens
ORM	Entity Framework Core 9.0
Database	SQL Server (LocalDB)
Identity	ASP.NET Core Identity (extended)
API Documentation	Swashbuckle / Swagger UI
Project Structure
text
CarRental.API/          ← Backend REST API (.NET 9)
CarRental.MVC/          ← Frontend MVC Application (.NET 9)
How to Run
1. Prerequisites
.NET 9.0 SDK
SQL Server / LocalDB
Visual Studio 2022/2025 or VS Code
2. Clone & Run
Bash
git clone https://github.com/yourusername/CarRental.git
cd CarRental
Open the solution in Visual Studio → Press F5 (both projects will start)

API runs on https://localhost:7xxx
MVC frontend runs on https://localhost:5xxx
3. Default Admin Account
Create the first user → it will be treated as admin in the system (or manually set IsAdmin = 1 in DB).

API Endpoints (selected)
Method	Endpoint	Description
POST	/api/Auth/register	Register new user
POST	/api/Auth/login	Login → returns JWT + refresh
POST	/api/Auth/refresh	Get new access token
POST	/api/Auth/revoke	Logout (revokes refresh token)
GET	/api/Cars	List available cars
POST	/api/Reservations	Create reservation
GET	/api/Admin/dashboard	Admin stats
GET	/api/Admin/users	List all users (admin only)
Full API available at /swagger when running.

Security Notes
Refresh tokens are stored securely and revoked on logout
Tokens are invalidated if used, revoked, or expired
Sensitive data never exposed (DTOs used everywhere)
Proper CORS and HTTPS redirection
License
MIT License – feel free to use, modify, and deploy commercially.

Made with love in Sweden in 2025
Happy coding!
