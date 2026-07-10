using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using projekt.Models;

namespace projekt.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for the projekt application.
    /// This class manages all database operations and entity mappings.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Device> Devices { get; set; }
        public DbSet<Calibration> Calibrations { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<Laboratory> Laboratories { get; set; }
        public DbSet<DeviceLocation> DeviceLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Device entity relationships
            modelBuilder.Entity<Device>()
                .HasMany(d => d.CalibrationHistory)
                .WithOne(c => c.Device)
                .HasForeignKey(c => c.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Device>()
                .HasMany(d => d.LocationHistory)
                .WithOne(dl => dl.Device)
                .HasForeignKey(dl => dl.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure DeviceLocation relationships
            modelBuilder.Entity<DeviceLocation>()
                .HasOne(dl => dl.Laboratory)
                .WithMany(l => l.DeviceLocations)
                .HasForeignKey(dl => dl.LaboratoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Calibration relationships with Technician
            modelBuilder.Entity<Calibration>()
                .HasOne(c => c.Technician)
                .WithMany(t => t.Calibrations)
                .HasForeignKey(c => c.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
