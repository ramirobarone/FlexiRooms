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

            modelBuilder.Entity<IssueType>().HasData(
                new IssueType { Id = 1, Issue = "Limpieza" },
                new IssueType { Id = 2, Issue = "Mantenimiento" },
                new IssueType { Id = 3, Issue = "Agua" },
                new IssueType { Id = 4, Issue = "Electricidad" },
                new IssueType { Id = 5, Issue = "Servicios" },
                new IssueType { Id = 6, Issue = "Ruido" },
                new IssueType { Id = 7, Issue = "Problema con el acceso" },
                new IssueType { Id = 8, Issue = "Otro" });
        }
    }
}
