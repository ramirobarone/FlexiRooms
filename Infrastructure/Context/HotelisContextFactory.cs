using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Context;

public class HotelisContextFactory : IDesignTimeDbContextFactory<RoomContainerContext>
{
    public RoomContainerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RoomContainerContext>();

        // Connection string used only at design time (migrations).
        // Replace with your local PostgreSQL credentials if needed.
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=roomContainer;Username=postgres;Password=!Jazmin1811;");

        return new RoomContainerContext(optionsBuilder.Options);
    }
}
