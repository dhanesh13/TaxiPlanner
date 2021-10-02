using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class Status
    {
        public string status { set; get; }
        public int booking_id { set; get; }
        public DateTime bookingslot_date_time { set; get; }
        public DateTime updated_date_time { set; get; }

    }

}