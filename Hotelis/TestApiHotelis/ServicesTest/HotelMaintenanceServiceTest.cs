using Application.Models;
using Application.Services.Maintenance;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace TestApiHotelis.ServicesTest;

public class HotelMaintenanceServiceTest
{
    private static FlexiRoomsContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FlexiRoomsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new FlexiRoomsContext(options);
        context.Database.EnsureCreated();

        if (!context.MaintenanceTypes.Any())
        {
            context.MaintenanceTypes.Add(new MaintenanceType
            {
                Id = 1,
                Description = "Electricidad"
            });
            context.SaveChanges();
        }

        if (!context.Hotels.Any(x => x.Id == 1))
        {
            context.Hotels.Add(new Hotel
            {
                Id = 1,
                Name = "Hotel Test"
            });
            context.SaveChanges();
        }

        return context;
    }

    [Fact]
    public async Task CreateAsync_InvalidHotelId_ThrowsArgumentException()
    {
        using var context = CreateContext();
        var service = new HotelMaintenanceService(context, NullLogger<HotelMaintenanceService>.Instance);

        var request = new CreateHotelMaintenanceDto
        {
            NameMaintenance = "Proveedor 1",
            HotelId = -1,
            MaintenanceTypeIds = [1],
            TelephoneNumber = "+54 11 1234 5678",
            Active = true
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request, "user-1"));
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedMaintenance()
    {
        using var context = CreateContext();
        var service = new HotelMaintenanceService(context, NullLogger<HotelMaintenanceService>.Instance);

        var request = new CreateHotelMaintenanceDto
        {
            NameMaintenance = "Proveedor 1",
            HotelId = 1,
            MaintenanceTypeIds = [1],
            TelephoneNumber = "+54 11 1234 5678",
            Active = true
        };

        HotelMaintenanceDto created = await service.CreateAsync(request, "user-1");

        Assert.Equal("Proveedor 1", created.NameMaintenance);
        Assert.Single(created.MaintenanceTypes);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesMaintenance()
    {
        using var context = CreateContext();
        var service = new HotelMaintenanceService(context, NullLogger<HotelMaintenanceService>.Instance);

        HotelMaintenanceDto created = await service.CreateAsync(new CreateHotelMaintenanceDto
        {
            NameMaintenance = "Proveedor 1",
            HotelId = 1,
            MaintenanceTypeIds = [1],
            TelephoneNumber = "+54 11 1234 5678",
            Active = true
        }, "user-1");

        HotelMaintenanceDto updated = await service.UpdateAsync(new UpdateHotelMaintenanceDto
        {
            Id = created.Id,
            NameMaintenance = "Proveedor 2",
            HotelId = 1,
            MaintenanceTypeIds = [1],
            TelephoneNumber = "+54 11 1234 0000",
            Active = false
        }, "user-2");

        Assert.Equal("Proveedor 2", updated.NameMaintenance);
    }
}
