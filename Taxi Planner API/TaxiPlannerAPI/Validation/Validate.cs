using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TaxiPlannerAPI.Models;
using TaxiPlannerAPI.Services;

namespace TaxiPlannerAPI.Validation
{
    public class Validate
    {
        public static Boolean ValidateBooking(DateTime date, int emp_id, TaxiPlannerAPIContext db)
        {
             //var users = Nucleus.GetEmployee(emp_id);

            return ((from b in db.Bookings
                     join bs in db.BookingSlots
                     on b.booking_id equals bs.booking_id
                     where b.emp_id == emp_id
                     //join u in users
                     //on b.emp_id equals u.user_id
                     where DbFunctions.DiffDays(bs.date_time, date) == 0
                     orderby bs.date_time descending
                     select bs).FirstOrDefault()) == null ? false : true;            
        }

       
    }
}