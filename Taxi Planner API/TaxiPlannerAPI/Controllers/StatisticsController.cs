using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TaxiPlannerAPI.Models;
using TaxiPlannerAPI.Services;


namespace TaxiPlannerAPI.Controllers
{
    public class StatisticsController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();

        [Authorizer]
        [Authorize(Roles = "approver, hr, superadmin")] //delegated approver does not see dashboard
        [HttpGet]
        public async Task<HttpResponseMessage> GetStatistics(int choice)
        {
            //choice 1= daily, 2=weekly  3=Dailychartforregions
            if (choice == 1)
            {
                DailyStatistics ds = new DailyStatistics();
                var total_bookings = 0;
                var total_bookings_pending = 0;
                var total_bookings_approved = 0;
                var total_bookings_rejected = 0;
                //hr and superadmin gets total count including their bookings as well
                if (Thread.CurrentPrincipal.IsInRole("hr") || Thread.CurrentPrincipal.IsInRole("superadmin"))
                {
                    total_bookings = (from bs in db.BookingSlots
                                      where (DbFunctions.DiffDays(bs.date_time, (DateTime.Now)) == 0)
                                      select bs).Count();

                    total_bookings_pending = (from bs in db.BookingSlots
                                              where (DbFunctions.DiffDays(bs.date_time, (DateTime.Now)) == 0) && bs.status == BookingStatus.PENDING
                                              select bs).Count();
                    total_bookings_approved = (from bs in db.BookingSlots
                                               where (DbFunctions.DiffDays(bs.date_time, (DateTime.Now)) == 0) && bs.status == BookingStatus.APPROVED
                                               select bs).Count();
                    total_bookings_rejected = (from bs in db.BookingSlots
                                               where (DbFunctions.DiffDays(bs.date_time, (DateTime.Now)) == 0) && bs.status == BookingStatus.REJECTED
                                               select bs).Count();
                }

                if (Thread.CurrentPrincipal.IsInRole("approver"))
                {
                    int approver_id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

                    var users = await Nucleus.GetEmployeedetails();

                    var repusers = (from u in users
                                    join us in users
                                    on u.superior_id equals us.user_id
                                    select new
                                    {
                                        u.user_id,
                                        us.superior_id
                                    }).ToList();


                    var bookings = (from bs in db.BookingSlots
                                    join b in db.Bookings
                                    on bs.booking_id equals b.booking_id
                                    where (DbFunctions.DiffDays(bs.date_time, (DateTime.Now)) == 0)
                                    select new { b.emp_id, b.booking_id, bs.date_time, bs.status }
                                   ).ToList();

                    total_bookings = (from b in bookings
                                      join u in repusers
                                      on b.emp_id equals u.user_id
                                      where u.superior_id == approver_id
                                      //&& (DbFunctions.DiffDays(b.date_time, (DateTime.Now)) == 0)
                                      select b.emp_id).Count();

                    total_bookings_pending = (from b in bookings
                                              join u in repusers
                                              on b.emp_id equals u.user_id
                                              where u.superior_id == approver_id
                                              //&& (DbFunctions.DiffDays(b.date_time, (DateTime.Now)) == 0)
                                              && b.status == BookingStatus.PENDING
                                              select b).Count();

                    total_bookings_approved = (from b in bookings
                                               join u in repusers
                                               on b.emp_id equals u.user_id


                                               where u.superior_id == approver_id
                                               //&& (DbFunctions.DiffDays(b.date_time, (DateTime.Now)) == 0)
                                               && b.status == BookingStatus.APPROVED
                                               select b).Count();

                    total_bookings_rejected = (from b in bookings
                                               join u in repusers
                                               on b.emp_id equals u.user_id


                                               where u.superior_id == approver_id
                                               //&& (DbFunctions.DiffDays(b.date_time, (DateTime.Now)) == 0)
                                               && b.status == BookingStatus.REJECTED
                                               select b).Count();
                }
                ds.total_num_bookings = total_bookings;
                ds.total_num_bookings_pending = total_bookings_pending;
                ds.total_num_bookings_approved = total_bookings_approved;
                ds.total_num_bookings_rejected = total_bookings_rejected;

                return Request.CreateResponse(HttpStatusCode.OK, ds);
            }
            else if (choice == 2)
            {
                if (Thread.CurrentPrincipal.IsInRole("hr") || Thread.CurrentPrincipal.IsInRole("superadmin"))
                {
                    var temp = from bs in db.BookingSlots
                               where DbFunctions.DiffDays(DateTime.Now, bs.date_time) > -7 &&  bs.date_time >= DateTime.Now
                               && bs.status == BookingStatus.APPROVED
                               group bs by DbFunctions.TruncateTime(bs.date_time);

                    var query = from t in temp
                                select new
                                {
                                    date_time = t.Key,
                                    totalSent = t.Count()
                                };
                    return Request.CreateResponse(HttpStatusCode.OK, query);
                }

                if (Thread.CurrentPrincipal.IsInRole("approver"))
                {
                    int approver_id = int.Parse(Thread.CurrentPrincipal.Identity.Name);


                    var users = await Nucleus.GetEmployeedetails();

                    var repusers = (from u in users
                                    join us in users
                                    on u.superior_id equals us.user_id
                                    select new
                                    {
                                        u.user_id,
                                        us.superior_id
                                    }).ToList();


                    var bookings = (from bs in db.BookingSlots
                                    join b in db.Bookings
                                    on bs.booking_id equals b.booking_id
                                    where DbFunctions.DiffDays(DateTime.Now, bs.date_time) > -7 && DbFunctions.DiffDays(DateTime.Now, bs.date_time) <= 0
                                    select new { b.emp_id, b.booking_id, bs.date_time, bs.status }
                                   ).ToList();

                    var temp = (from b in bookings
                               join u in repusers
                               on b.emp_id equals u.user_id
                               where u.superior_id == approver_id
                               //&& DbFunctions.DiffDays(DateTime.Now, b.date_time) > -7 && DbFunctions.DiffDays(DateTime.Now, b.date_time) <= 0
                               group b by b.date_time).ToList();

                    var query = from t in temp
                                select new
                                {
                                    date_time = t.Key,
                                    totalSent = t.Count()
                                };
                    return Request.CreateResponse(HttpStatusCode.OK, query.ToList());
                }
            }

            //Choice = 3 hr barchart
            else if (choice == 3)
            {
                if (Thread.CurrentPrincipal.IsInRole("hr") || Thread.CurrentPrincipal.IsInRole("superadmin")|| Thread.CurrentPrincipal.IsInRole("approver"))
                {
                    var users = await Nucleus.GetEmployeedetails();

                    var repusers = (from u in users
                                    join us in users
                                    on u.superior_id equals us.user_id
                                    select new
                                    {
                                        u.region,
                                        u.user_id,
                                        us.superior_id
                                    }).ToList();


                    var bookings = (from bs in db.BookingSlots
                                    join b in db.Bookings
                                    on bs.booking_id equals b.booking_id
                                    select new { b.emp_id, b.booking_id, bs.date_time, bs.status }
                                   ).ToList();

                    var regions = (from r in db.Regions
                                   select new
                                   {
                                       r.region_name,
                                       r.estimated_price
                                   }).ToList();

                    var temp = from b in bookings
                               join u in repusers
                               on b.emp_id equals u.user_id
                               join r in regions
                               on u.region equals r.region_name
                               where b.date_time ==DateTime.Now 
                               group r by r.region_name;

                    var query = (from t in temp
                                 select new
                                 {
                                     region_name = t.Key,
                                     dailyregionbookings = t.Count()
                                 }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, query);
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "You dont have permission to view this");
        }
    }
}

///*
//https://stackoverflow.com/questions/3101824/groupby-in-lambda-expressions
//https://stackoverflow.com/questions/50367637/sql-count-in-c-sharp-lambda-expression
//SELECT COUNT(booking_id) as total, CAST(date_time AS date) as date
//from BookingSlots
//group by CAST(date_time AS date)
//*/
