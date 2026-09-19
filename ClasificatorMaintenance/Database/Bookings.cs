using Microsoft.Extensions.Primitives;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClasificatorMaintenance.Database
{
    public class Bookings
    {
        public int Id { get; set; }
        public int IdRoom { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime DateReserved { get; set; }
    }
}