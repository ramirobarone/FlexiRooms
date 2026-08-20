using Microsoft.Extensions.Primitives;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Bookings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CheckInTimeId { get; set; }
        public int IdRoom { get; set; }
        public decimal Price { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime DateReserved { get; set; }
        //public int IdPayment { get; set; }
        //public string? DatePaymentCreated { get; set; }
        //public string? DatePaymentApproved { get; set; }
        //public bool? IsPaymentApproved { get; set; }
        //public Guid IdemPotencyKey { get; set; }


        public virtual TimesAvailable? CheckInTime { get; set; }
        public virtual User? User { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id} - {nameof(DateReserved)}:{DateReserved} - {nameof(CheckInTime)}:{CheckInTime?.Time} - {nameof(IdRoom)}:{IdRoom} -{nameof(Price)}: {Price} - {nameof(User.Id)}:{User?.Id}";
        }
    }
    public class DetailPayment
    {
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? XIdempotencyKey { get; set; }
        public decimal Amount { get; set; }
        public string? IdTransaccion {  get; set; }
        public string? StateTransaction { get; set; }


    }
}