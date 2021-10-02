using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TaxiPlannerAPI.Data;
using TaxiPlannerAPI.Models;

namespace TaxiPlannerAPI.Services
{
    public class AllocationController : ApiController
    {
        private static TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();
        public static async Task<List<Transport>> GenerateAll()
        {
            List<Transport> transports = new List<Transport>();
            for (int i = 1; i < 4; i++)
            {
                List<Transport> temp_transports = await AllocatePerSchedule(i);
                temp_transports.ForEach(t => transports.Add(t));
            }
            return transports;
        }

        public static async Task<List<Transport>> AllocatePerSchedule(int schedule)
        {
            // schedules:
            // 1 - 19:30
            // 2 - 20:30
            // else - 21:30

            int hours;
            int minutes = 30;

            switch (schedule)
            {
                case 1:
                    hours = 19;
                    break;
                case 2:
                    hours = 20;
                    break;
                default:
                    hours = 21;
                    break;
            }

            DateTime dt = DateTime.Today;
            TimeSpan ts = new TimeSpan(hours, minutes, 0);
            dt += ts;

            List<Transport> transports = new List<Transport>();

            // employees
            List<EmployeeDTO> employees = await Nucleus.GetEmployeedetails();
            // join with bookingslots for today
            var bookingSlots = from b in ((from b in db.Bookings
                                           join bs in db.BookingSlots
                                           on b.booking_id equals bs.booking_id
                                           where bs.date_time == dt
                                           where bs.status == BookingStatus.APPROVED
                                           select b
                               ).ToList())
                               join e in employees
                   on b.emp_id equals e.user_id
                               select new
                               {
                                   e.sub_regions,
                                   e.region,
                                   e.user_id
                               };

            if (bookingSlots.ToList().Count == 0)
                return transports;

            var subregion_query = from e in bookingSlots
                                  group new { e.sub_regions, e.region } by e.sub_regions;

            var subregions = from t in subregion_query
                             select new Subregion()
                             {
                                 sub_regions = t.Key,
                                 region = t.FirstOrDefault().region,
                                 count = t.Count()
                             };
            var subregion_count_list = subregions.OrderByDescending(s => s.count);

            foreach (var subregion_item in subregion_count_list)
            {
                // subregion_count_list is in desc order
                // to improve efficency, break on encountering first item where count < 7
                if (subregion_item.count < 7)
                    break;

                // allocate passengers in sub regions where all seats will be booked
                while (subregion_item.count >= 14)
                {
                    int count = subregion_item.count >= 14 ? 14 : subregion_item.count;
                    subregion_item.count -= count;
                    Transport newTransport = new Transport()
                    {
                        passengers_count = count,
                        region = subregion_item.region,
                        timestamp = dt,
                    };
                    newTransport.subregions.Add(subregion_item.sub_regions);
                    transports.Add(newTransport);
                }

                // group passengers from remaining sub regions where count > 7
                if (subregion_item.count >= 7)
                {
                    Transport newTransport = new Transport()
                    {
                        passengers_count = subregion_item.count,
                        region = subregion_item.region,
                        timestamp = dt,
                    };
                    subregion_item.count = 0;
                    newTransport.subregions.Add(subregion_item.sub_regions);
                    transports.Add(newTransport);
                }
            }

            // remove subregions where all employees have been allocated
            subregion_count_list = subregion_count_list
                .Where(sr => sr.count != 0)
                .OrderBy(sr => sr.region);

            // we are left with a list of passengers which can be regrouped
            Transport transport = new Transport()
            {
                timestamp = dt,
            };

            foreach (var subregion_item in subregion_count_list)
            {
                // get another transport when the threshold is reached
                // or all passengers from a subregion have been already been allocated
                if (transport.passengers_count >= 7 || transport.region != null && transport.region != subregion_item.region)
                {
                    transports.Add(transport);
                    transport = new Transport()
                    {
                        timestamp = dt,
                    };
                }

                transport.region = subregion_item.region;
                transport.subregions.Add(subregion_item.sub_regions);
                transport.passengers_count += subregion_item.count;
            }
            transports.Add(transport);

            // sort by subregion to ensure that vans with full capacity get are filled first
            var sortedEmp = (from e in employees
                             join b in bookingSlots
                             on e.user_id equals b.user_id
                             select e
                             )
                             .ToList()
                             .OrderBy(e => e.sub_regions);
            //.OrderBy(e => e.region);
            // allocate passengers to transport
            foreach (var employee in sortedEmp)
            {
                foreach (var t in transports)
                {
                    if (t.passengers.Count() < t.passengers_count && t.subregions.Contains(employee.sub_regions))
                    {
                        t.passengers.Add(employee);
                        break;
                    }
                }
            }

            return transports;
        }

    }
}
