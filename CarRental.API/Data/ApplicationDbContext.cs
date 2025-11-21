using CarRental.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CarRental.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Car> Cars { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Car>(entity =>
            {
                entity.HasIndex(e => e.LicensePlate).IsUnique();
                entity.Property(e => e.PricePerDay).HasColumnType("decimal(18,2)");
            });
            
            builder.Entity<Reservation>(entity =>
            {
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Car)
                    .WithMany(c => c.Reservations)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reservations)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.Token).IsUnique();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
