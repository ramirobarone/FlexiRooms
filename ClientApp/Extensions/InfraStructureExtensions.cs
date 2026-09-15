using ClientApp.OptionsPattern;
using Infrastructure.Models;
using Infrastructure.Repository;
using MercadoPago.Client.Payment;
using MercadoPago.Config;

namespace ClientApp.Extensions
{
    public static class InfraStructureExtensions
    {
        public static void AddInfraStructure(this WebApplicationBuilder webApplication)
        {
            webApplication.Services.AddScoped<IRepository<Hotel>, Repository<Hotel>>();
            webApplication.Services.AddScoped<IRepository<HotelPicture>, Repository<HotelPicture>>();
            webApplication.Services.AddScoped<IRepository<Room>, Repository<Room>>();
            webApplication.Services.AddScoped<IRepository<Cost>, Repository<Cost>>();
            webApplication.Services.AddScoped<IRepository<Bookings>, Repository<Bookings>>();
            webApplication.Services.AddScoped<IRepository<TimesAvailable>, Repository<TimesAvailable>>();
            webApplication.Services.AddScoped<IRepository<User>, Repository<User>>();
            webApplication.Services.AddScoped<IRepository<PaymentTransaction>, Repository<PaymentTransaction>>();
            webApplication.Services.AddScoped<IRepository<HotelInfo>, Repository<HotelInfo>>();

            MercadoPagoOption mercadoPagoOption = new();
            webApplication.Configuration.GetSection(MercadoPagoOption.MercadoPagoOptionName).Bind(mercadoPagoOption);
            MercadoPagoConfig.AccessToken = mercadoPagoOption.Token ?? throw new Exception("Token mercado pago no existe");

            webApplication.Services.AddScoped<PaymentClient>();
        }
    }
}
