using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class Booking
    {
        [Key]
        public int booking_id { get; set; }
        public DateTime timestamp_booked { get; set; }

        public ICollection<BookingSlot> Bookingslots { get; set; }

        //Foreign key for Employee
        public int emp_id { get; set; }
        
        
    }
}