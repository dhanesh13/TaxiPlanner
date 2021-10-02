using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class ScheduleTime
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [Column(Order = 0)]
        public int schedule_type { set; get; }

        [Key]
        [Column(Order = 1)]
        public DateTime time { get; set; }

        public string schedule_name { get; set; }
    }
}

//if Schedule type is 0: Times a person can choose for his bookings
//1: deadline which he can make a booking in a day
//etc