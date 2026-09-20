using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.CheckOut
{
    public class WebHookPaymentCreatedDto
    {
        public string Action { get; set; }
        public string Api_version { get; set; }
        public Data Data { get; set; }
        public DateTime Date_created { get; set; }
        public long Id { get; set; }
        public bool Live_mode { get; set; }
        public string Type { get; set; }
        public string User_id { get; set; }
    }

    public class Data
    {
        public string Id { get; set; }
    }

}
