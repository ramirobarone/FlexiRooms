using Application.Interfaces;
using Application.Models;
using Application.Models.Options;
using Application.Services.Account;
using Application.Services.HotelServices;
using Application.Services.Reserves;
using Application.Services.Rooms;
using Infrastructure.Models;

namespace ClientApp.Extensions
{
    public static class ApplicationExtensions
    {
        public static void AddApplication(this WebApplicationBuilder app)
        {
            app.Services.AddOptions<JwtOptions>().BindConfiguration("JwtOptions").ValidateOnStart();

            app.Services.AddScoped<IHotelService, HotelServiceQuery>();
            app.Services.AddScoped<IHotelImageService, HotelImageService>();
            app.Services.AddScoped<IRoomImageService, RoomImageService>();
            app.Services.AddScoped<IServiceGeneric<HotelDto>, HotelServiceQuery>();
            app.Services.AddScoped<IServiceGeneric<Bookings>, BookingService>();
            app.Services.AddScoped<IServiceSearchByKeyword<HotelDto>, HotelServiceQuery>();
            app.Services.AddScoped<IServiceGeneric<RoomDto>, ServiceRoom>();
            app.Services.AddScoped<IBookings, BookingService>();
            app.Services.AddScoped<IServiceGeneric<RoomDto>, ServiceRoom>();
            app.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            app.Services.AddScoped<IAccountService, AccountService>();

        }
    }
}