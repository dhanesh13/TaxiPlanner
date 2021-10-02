using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using TaxiPlannerAPI.Models;
using TaxiPlannerAPI.Services;
using System.Threading.Tasks;

namespace TaxiPlannerAPI.Controllers
{
    public class ReportController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();

        [Authorizer]
        [Authorize(Roles = "superadmin, hr")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetReportValues(DateTime dateFrom, DateTime dateTo)
        {
            var users = await Nucleus.GetEmployeedetails();

            //var regions = (from r in db.Regions
            //              select new
            //              {
            //                  //r.region_id,
            //                  r.region_name,
            //                  r.estimated_price
            //              }).ToList();

            var repusers = (from u in users
                            join us in users
                            on u.superior_id equals us.user_id
                            select new
                            {
                                u.region,
                                u.user_id,
                                u.user_name,
                                u.superior_id,
                                u.superior_name,
                                u.segment_name,
                                u.costcenter_name
                            }).ToList();


            var bookings = (from bs in db.BookingSlots
                            join b in db.Bookings
                            on bs.booking_id equals b.booking_id
                            select new { b.emp_id, b.booking_id, bs.date_time, bs.status }
                           ).ToList();

            var temp = (from b in bookings
                       join u in repusers
                       on b.emp_id equals u.user_id
                       where b.date_time >= dateFrom && b.date_time <= dateTo && b.status == BookingStatus.APPROVED
                       group new { b.emp_id, name = u.user_name, u.region, u.costcenter_name, u.segment_name, reportTo = u.superior_name }by u.user_id).ToList();

            //var temp = from BS in db.BookingSlots
            //           join book in db.Bookings
            //           on BS.booking_id equals book.booking_id
            //           join e in db.Employees
            //           on book.emp_id equals e.emp_id
            //           join r in db.Regions
            //           on e.region_id equals r.region_id
            //           join ct in db.CostCenters
            //           on e.costcenter_id equals ct.costcenter_id
            //           join dpt in db.Departments
            //           on ct.dept_id equals dpt.dept_id
            //           join e2 in db.Employees
            //           on e.reporting_line_id equals e2.emp_id
            //           where BS.date_time >= dateFrom && BS.date_time <= dateTo && BS.status == BookingStatus.APPROVED
            //           group new { book.emp_id, e.name, r.region_name, r.estimated_price, ct.costcenter_name, dpt.dept_name, reportTo = e2.name } by e.emp_id;

            var query = from t in temp
                        select new
                        {
                            emp_id = t.Key,
                            t.FirstOrDefault().name,
                            t.FirstOrDefault().region,
                            t.FirstOrDefault().costcenter_name,
                            t.FirstOrDefault().segment_name,
                            travel_count = t.Count(),
                            //travel_amount = t.Count() * t.FirstOrDefault().estimated_price,

                            direct_superior = t.FirstOrDefault().reportTo
                        };

            return Request.CreateResponse(HttpStatusCode.OK, query);
        }
    }
}
