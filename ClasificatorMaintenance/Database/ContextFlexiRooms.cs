using Microsoft.EntityFrameworkCore;

namespace ClasificatorMaintenance.Database;

internal sealed class ContextFlexiRooms : DbContext
{
    public ContextFlexiRooms()
    {
    }

    internal DbSet<Issue> Issues { get; set; } = null!;
    internal DbSet<Hotel> Hotels { get; set; } = null!;
    internal DbSet<MaintenanceType> MaintenanceTypes { get; set; } = null!; 
    internal DbSet<Bookings> Bookings { get; set; } = null!;

    public ContextFlexiRooms(DbContextOptions<ContextFlexiRooms> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=flexirooms;Username=postgres;Password=postgres;");
    }
}
