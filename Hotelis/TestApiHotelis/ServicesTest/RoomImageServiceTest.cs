using Application.Models;
using Application.Services.Rooms;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace TestApiHotelis.ServicesTest;

public class RoomImageServiceTest
{
    private static FlexiRoomsContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FlexiRoomsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new FlexiRoomsContext(options);
    }

    [Fact]
    public async Task UploadImagesAsync_InvalidRoomId_ThrowsArgumentException()
    {
        using var context = CreateContext();
        var service = new RoomImageService(context, NullLogger<RoomImageService>.Instance);

        var files = new List<RoomImageUploadFile>
        {
            new("test.jpg", new byte[] { 1, 2, 3 })
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.UploadImagesAsync(0, files));
    }

    [Fact]
    public async Task UploadImagesAsync_EmptyFiles_ThrowsArgumentException()
    {
        using var context = CreateContext();
        var service = new RoomImageService(context, NullLogger<RoomImageService>.Instance);

        var files = new List<RoomImageUploadFile>();

        await Assert.ThrowsAsync<ArgumentException>(() => service.UploadImagesAsync(1, files));
    }

    [Fact]
    public async Task GetRoomImagesAsync_InvalidRoomId_ThrowsArgumentException()
    {
        using var context = CreateContext();
        var service = new RoomImageService(context, NullLogger<RoomImageService>.Instance);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetRoomImagesAsync(-1));
    }

    [Fact]
    public async Task UploadImagesAsync_RoomNotFound_ThrowsInvalidOperationException()
    {
        using var context = CreateContext();
        var service = new RoomImageService(context, NullLogger<RoomImageService>.Instance);

        var files = new List<RoomImageUploadFile>
        {
            new("test.jpg", new byte[] { 1, 2, 3 })
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadImagesAsync(999, files));
    }
}
