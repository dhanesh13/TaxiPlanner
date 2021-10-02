using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class TaxiPlannerAPIContext : DbContext
    {


        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public TaxiPlannerAPIContext() : base("name=TaxiPlannerAPIContext")
        {
        }


        public System.Data.Entity.DbSet<TaxiPlannerAPI.Models.Region> Regions { get; set; }

        public System.Data.Entity.DbSet<TaxiPlannerAPI.Models.BookingSlot> BookingSlots { get; set; }
        public System.Data.Entity.DbSet<TaxiPlannerAPI.Models.Booking> Bookings { get; set; }
        public System.Data.Entity.DbSet<TaxiPlannerAPI.Models.Role> Roles { get; set; }
        public System.Data.Entity.DbSet<TaxiPlannerAPI.Models.ScheduleTime> ScheduleTimes { get; set; }

        public System.Data.Entity.DbSet<TaxiPlannerAPI.Models.NotificationToken> NotificationTokens { get; set; }
    }
}
