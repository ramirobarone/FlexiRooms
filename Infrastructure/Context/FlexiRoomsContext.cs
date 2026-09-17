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
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }


    public DbSet<HotelPicture> HotelPicture { get; set; }
    public DbSet<UserProfileImage> UserProfileImages { get; set; }
    public DbSet<HotelInfo> HotelInfos { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<IssueType> IssuesTypes { get; set; }
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

        modelBuilder.Entity<UserProfileImage>()
            .HasKey(image => image.UserId);

        modelBuilder.Entity<UserProfileImage>()
            .HasOne(image => image.User)
            .WithOne()
            .HasForeignKey<UserProfileImage>(image => image.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(payment => payment.MercadoPagoPaymentId)
            .IsUnique();

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(payment => payment.IdempotencyKey)
            .IsUnique();

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(payment => payment.ApplicationUser)
            .WithMany(user => user.PaymentTransactions)
            .HasForeignKey(payment => payment.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(payment => payment.Booking)
            .WithMany()
            .HasForeignKey(payment => payment.BookingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HotelInfo>()
            .HasOne(hotelInfo => hotelInfo.Hotel)
            .WithMany()
            .HasForeignKey(hotelInfo => hotelInfo.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Issue>()
            .HasOne(issue => issue.IssueType)
            .WithMany()
            .HasForeignKey(issue => issue.TipoDeReclamo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Issue>()
            .HasOne(issue => issue.ApplicationUser)
            .WithMany()
            .HasForeignKey(issue => issue.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
