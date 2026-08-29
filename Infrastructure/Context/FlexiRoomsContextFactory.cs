using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Context;

public class FlexiRoomsContextFactory : IDesignTimeDbContextFactory<FlexiRoomsContext>
{
    public FlexiRoomsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FlexiRoomsContext>();

        // Connection string used only at design time (migrations).
        // Replace with your local PostgreSQL credentials if needed.
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=roomContainer;Username=postgres;Password=!Jazmin1811;");

        return new FlexiRoomsContext(optionsBuilder.Options);
    }
}
