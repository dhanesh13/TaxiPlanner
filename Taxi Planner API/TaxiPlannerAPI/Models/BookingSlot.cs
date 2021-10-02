using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class BookingSlot
    {
        [Key]
        [Column(Order =0)]
        public int booking_id { get; set; }
        public Booking Bookings { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime date_time { get; set; }

        public BookingStatus status { get; set; }

        public string reason { get; set; }

        public DateTime? evaluation_timestamp { get; set; }
        //added ? in order to make it nullable value

        public int? approved_by_id { get; set; }  
        

        //this variable is not mapped to the database 
        //and is used only to set the values of empId when Hr and Manager makes bookings
        [NotMapped]
        public int emp_id { get; set; }

    }

    public enum BookingStatus
    {
        PENDING,
        APPROVED,
        REJECTED
    }
}