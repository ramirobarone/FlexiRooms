using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public static class SeedData
    {
        public static void SeedDataHotelis(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Argentina" },
                new Country { Id = 2, Name = "Chile" },
                new Country { Id = 3, Name = "Bolivia" },
                new Country { Id = 4, Name = "Brasil" },
                new Country { Id = 5, Name = "Uruguay" });

            modelBuilder.Entity<Province>().HasData(
                new { Id = 1, Name = "Buenos Aires", CountryId = 1 },
                new { Id = 2, Name = "Catamarca", CountryId = 1 },
                new { Id = 3, Name = "Chaco", CountryId = 1 },
                new { Id = 4, Name = "Chubut", CountryId = 1 },
                new { Id = 5, Name = "Córdoba", CountryId = 1 },
                new { Id = 6, Name = "Corrientes", CountryId = 1 },
                new { Id = 7, Name = "Entre Ríos", CountryId = 1 },
                new { Id = 8, Name = "Formosa", CountryId = 1 },
                new { Id = 9, Name = "Jujuy", CountryId = 1 },
                new { Id = 10, Name = "La Pampa", CountryId = 1 },
                new { Id = 11, Name = "La Rioja", CountryId = 1 },
                new { Id = 12, Name = "Mendoza", CountryId = 1 },
                new { Id = 13, Name = "Misiones", CountryId = 1 },
                new { Id = 14, Name = "Neuquén", CountryId = 1 },
                new { Id = 15, Name = "Río Negro", CountryId = 1 },
                new { Id = 16, Name = "Salta", CountryId = 1 },
                new { Id = 17, Name = "San Juan", CountryId = 1 },
                new { Id = 18, Name = "San Luis", CountryId = 1 },
                new { Id = 19, Name = "Santa Cruz", CountryId = 1 },
                new { Id = 20, Name = "Santa Fe", CountryId = 1 },
                new { Id = 21, Name = "Santiago del Estero", CountryId = 1 },
                new { Id = 22, Name = "Tierra del Fuego", CountryId = 1 },
                new { Id = 23, Name = "Tucumán", CountryId = 1 },
                new { Id = 24, Name = "Ciudad Autónoma de Buenos Aires", CountryId = 1 });

            modelBuilder.Entity<City>().HasData(
                new { Id = 1, Name = "Buenos Aires", ProvinceId = 24 },
                new { Id = 2, Name = "La Plata", ProvinceId = 1 },
                new { Id = 3, Name = "Mar del Plata", ProvinceId = 1 },
                new { Id = 4, Name = "Bahía Blanca", ProvinceId = 1 },
                new { Id = 5, Name = "San Fernando del Valle de Catamarca", ProvinceId = 2 },
                new { Id = 6, Name = "Resistencia", ProvinceId = 3 },
                new { Id = 7, Name = "Comodoro Rivadavia", ProvinceId = 4 },
                new { Id = 8, Name = "Córdoba", ProvinceId = 5 },
                new { Id = 9, Name = "Corrientes", ProvinceId = 6 },
                new { Id = 10, Name = "Paraná", ProvinceId = 7 },
                new { Id = 11, Name = "Formosa", ProvinceId = 8 },
                new { Id = 12, Name = "San Salvador de Jujuy", ProvinceId = 9 },
                new { Id = 13, Name = "Santa Rosa", ProvinceId = 10 },
                new { Id = 14, Name = "La Rioja", ProvinceId = 11 },
                new { Id = 15, Name = "Mendoza", ProvinceId = 12 },
                new { Id = 16, Name = "Posadas", ProvinceId = 13 },
                new { Id = 17, Name = "Neuquén", ProvinceId = 14 },
                new { Id = 18, Name = "San Carlos de Bariloche", ProvinceId = 15 },
                new { Id = 19, Name = "Salta", ProvinceId = 16 },
                new { Id = 20, Name = "San Juan", ProvinceId = 17 },
                new { Id = 21, Name = "San Luis", ProvinceId = 18 },
                new { Id = 22, Name = "Río Gallegos", ProvinceId = 19 },
                new { Id = 23, Name = "Rosario", ProvinceId = 20 },
                new { Id = 24, Name = "Santa Fe", ProvinceId = 20 },
                new { Id = 25, Name = "Santiago del Estero", ProvinceId = 21 },
                new { Id = 26, Name = "Ushuaia", ProvinceId = 22 },
                new { Id = 27, Name = "San Miguel de Tucumán", ProvinceId = 23 },
                new { Id = 28, Name = "Puerto Iguazú", ProvinceId = 13 },
                new { Id = 29, Name = "Villa Carlos Paz", ProvinceId = 5 },
                new { Id = 30, Name = "El Calafate", ProvinceId = 19 });

            modelBuilder.Entity<TimesAvailable>().HasData(
                new { Id = 1, Time = "08:00" },
                new { Id = 2, Time = "10:00" },
                new { Id = 3, Time = "12:00" },
                new { Id = 4, Time = "14:00" },
                new { Id = 5, Time = "16:00" },
                new { Id = 6, Time = "18:00" },
                new { Id = 7, Time = "20:00" },
                new { Id = 8, Time = "22:00" });

            modelBuilder.Entity<Cost>().HasData(
                new Cost { Id = 1, Hour = 2, CostPerTime = 45000m },
                new Cost { Id = 2, Hour = 4, CostPerTime = 78000m },
                new Cost { Id = 3, Hour = 8, CostPerTime = 120000m },
                new Cost { Id = 4, Hour = 12, CostPerTime = 165000m },
                new Cost { Id = 5, Hour = 2, CostPerTime = 52000m },
                new Cost { Id = 6, Hour = 4, CostPerTime = 86000m },
                new Cost { Id = 7, Hour = 8, CostPerTime = 134000m },
                new Cost { Id = 8, Hour = 12, CostPerTime = 182000m });

            modelBuilder.Entity<Address>().HasData(
                new Address { Id = 1, Street = "Av. Corrientes", Number = "1234", PostalCode = "C1043", Latitud = "-34.6037", Longitud = "-58.3816", IdCity = 1 },
                new Address { Id = 2, Street = "Bv. Oroño", Number = "890", PostalCode = "S2000", Latitud = "-32.9442", Longitud = "-60.6505", IdCity = 23 },
                new Address { Id = 3, Street = "Av. San Martín", Number = "456", PostalCode = "M5500", Latitud = "-32.8895", Longitud = "-68.8458", IdCity = 15 },
                new Address { Id = 4, Street = "Av. Bustillo", Number = "11500", PostalCode = "R8400", Latitud = "-41.1335", Longitud = "-71.3103", IdCity = 18 },
                new Address { Id = 5, Street = "Caseros", Number = "786", PostalCode = "A4400", Latitud = "-24.7829", Longitud = "-65.4232", IdCity = 19 });

            modelBuilder.Entity<Hotel>().HasData(
                new { Id = 1, CodeArea = 11, PhoneNumber = 43219876, Name = "Hotel Obelisco", Description = "Hotel urbano en el centro porteño.", MetaDescription = "Ideal para negocios y escapadas en Buenos Aires.", Email = "reservas@hotelobelisco.com", AddressHotelId = 1 },
                new { Id = 2, CodeArea = 341, PhoneNumber = 5588776, Name = "Rosario Riverside", Description = "Hotel moderno con vista al río Paraná.", MetaDescription = "Alojamiento premium en Rosario.", Email = "hola@rosarioriverside.com", AddressHotelId = 2 },
                new { Id = 3, CodeArea = 261, PhoneNumber = 4477551, Name = "Andes Suites Mendoza", Description = "Suites boutique cerca de bodegas y montaña.", MetaDescription = "Descanso y vino en Mendoza.", Email = "info@andessuites.com", AddressHotelId = 3 },
                new { Id = 4, CodeArea = 294, PhoneNumber = 4522334, Name = "Patagonia View Bariloche", Description = "Hotel de montaña con vista al lago.", MetaDescription = "Experiencia patagónica en Bariloche.", Email = "contacto@patagoniaview.com", AddressHotelId = 4 },
                new { Id = 5, CodeArea = 387, PhoneNumber = 4123456, Name = "Salta Colonial", Description = "Hotel cálido en el casco histórico salteño.", MetaDescription = "Tradición y confort en Salta.", Email = "reservas@saltacolonial.com", AddressHotelId = 5 });

            modelBuilder.Entity<HotelPicture>().HasData(
                new { Id = 1, Path = "assets/hotels/obelisco.jpg", HotelId = 1 },
                new { Id = 2, Path = "assets/hotels/rosario-riverside.jpg", HotelId = 2 },
                new { Id = 3, Path = "assets/hotels/andes-suites.jpg", HotelId = 3 },
                new { Id = 4, Path = "assets/hotels/patagonia-view.jpg", HotelId = 4 },
                new { Id = 5, Path = "assets/hotels/salta-colonial.jpg", HotelId = 5 });

            modelBuilder.Entity<Room>().HasData(
                new { Id = 1, BedNumbers = 1, AvialableNow = true, Name = "Single Business", Description = "Habitación individual con escritorio.", HotelsId = 1, CostId = 1 },
                new { Id = 2, BedNumbers = 2, AvialableNow = true, Name = "Doble Ejecutiva", Description = "Habitación doble con desayuno incluido.", HotelsId = 1, CostId = 2 },
                new { Id = 3, BedNumbers = 2, AvialableNow = true, Name = "Suite Paraná", Description = "Suite con vista al río.", HotelsId = 2, CostId = 6 },
                new { Id = 4, BedNumbers = 3, AvialableNow = false, Name = "Familiar Rosario", Description = "Ideal para familias y estadías cortas.", HotelsId = 2, CostId = 7 },
                new { Id = 5, BedNumbers = 2, AvialableNow = true, Name = "Suite Malbec", Description = "Habitación premium con ambientación mendocina.", HotelsId = 3, CostId = 6 },
                new { Id = 6, BedNumbers = 4, AvialableNow = true, Name = "Familiar Cordillera", Description = "Amplia habitación para grupos pequeños.", HotelsId = 3, CostId = 8 },
                new { Id = 7, BedNumbers = 2, AvialableNow = true, Name = "Lago Superior", Description = "Vista al lago y detalles patagónicos.", HotelsId = 4, CostId = 7 },
                new { Id = 8, BedNumbers = 3, AvialableNow = true, Name = "Suite Nahuel", Description = "Suite con living y balcón.", HotelsId = 4, CostId = 8 },
                new { Id = 9, BedNumbers = 1, AvialableNow = true, Name = "Colonial Single", Description = "Opción práctica para viajeros solos.", HotelsId = 5, CostId = 1 },
                new { Id = 10, BedNumbers = 2, AvialableNow = true, Name = "Tradición Norteña", Description = "Decoración regional y patio interno.", HotelsId = 5, CostId = 5 });

            modelBuilder.Entity<RoomPicture>().HasData(
                new { Id = 1, Name = "assets/rooms/single-business.jpg", RoomId = 1 },
                new { Id = 2, Name = "assets/rooms/doble-ejecutiva.jpg", RoomId = 2 },
                new { Id = 3, Name = "assets/rooms/suite-parana.jpg", RoomId = 3 },
                new { Id = 4, Name = "assets/rooms/familiar-rosario.jpg", RoomId = 4 },
                new { Id = 5, Name = "assets/rooms/suite-malbec.jpg", RoomId = 5 },
                new { Id = 6, Name = "assets/rooms/familiar-cordillera.jpg", RoomId = 6 },
                new { Id = 7, Name = "assets/rooms/lago-superior.jpg", RoomId = 7 },
                new { Id = 8, Name = "assets/rooms/suite-nahuel.jpg", RoomId = 8 },
                new { Id = 9, Name = "assets/rooms/colonial-single.jpg", RoomId = 9 },
                new { Id = 10, Name = "assets/rooms/tradicion-nortena.jpg", RoomId = 10 });
        }
    }
}
