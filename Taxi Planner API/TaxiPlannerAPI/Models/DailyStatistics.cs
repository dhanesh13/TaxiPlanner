using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class DailyStatistics
    {
        public int total_num_bookings { get; set; }
        public int total_num_bookings_pending { get; set; }
        public int total_num_bookings_approved { get; set; }
        public int total_num_bookings_rejected { get; set; }
    }
}