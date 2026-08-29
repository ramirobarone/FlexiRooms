using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class FlexiRoomsContext : IdentityDbContext<ApplicationUser>
{
    public FlexiRoomsContext()
    {

    }
    public FlexiRoomsContext(DbContextOptions<FlexiRoomsContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Address { get; set; }
    public DbSet<TimesAvailable> TimesAvialable { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Cost> Costs { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Province> Provincies { get; set; }
    public DbSet<Bookings> Bookings { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomPicture> RoomPictures { get; set; }
    //public DbSet<User> Users { get; set; }
    public DbSet<PreBooking> PreBooking { get; set; }


    public DbSet<HotelPicture> HotelPicture { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseNpgsql("Host=localhost;Database=hotelis;Username=hotelis;Password=Hotelis2024;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.SeedDataHotelis();

        modelBuilder.Entity<Hotel>()
            .HasOne(h => h.Owner)
            .WithMany()
            .HasForeignKey(h => h.IdentityNumber)
            .HasPrincipalKey(u => u.Id)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
