namespace Application.Models.CheckOut
{
    public class RequestPayment
    {
 
        public Additional_Info? additional_info { get; set; }
        public object? application_fee { get; set; }
        public bool binary_mode { get; set; } = true;
        public object? campaign_id { get; set; }
        public bool capture { get; set; }
        public object? coupon_amount { get; set; }
        public string? description { get; set; }
        public object? differential_pricing_id { get; set; }
        public string? external_reference { get; set; }
        public int installments { get; set; }
        public object? metadata { get; set; }
        public Payer1? payer { get; set; }
        public string? payment_method_id { get; set; }
        public string? token { get; set; }
        public int transaction_amount { get; set; }
    }
    public class Phone
    {
        public int area_code { get; set; }
        public String? number { get; set; }
    }

    public class Address
    {
        public object? street_number { get; set; }
    }
    public class Category_Descriptor
    {
        public Passenger? passenger { get; set; }
        public Route? route { get; set; }
    }

    public class Passenger
    {
    }

    public class Route
    {
    }
}