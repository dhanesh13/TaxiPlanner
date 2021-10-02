using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TaxiPlannerAPI.Models;

namespace TaxiPlannerAPI.Controllers
{
    public class ScheduleTimesController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();

        // GET: api/ScheduleTimes
        [Authorizer]
        [Authorize]
        public IQueryable<ScheduleTime> GetScheduleTimes()
        {
            return db.ScheduleTimes;
        }

        // GET: api/ScheduleTimes/5
        [Authorizer]
        [Authorize]
        [ResponseType(typeof(ScheduleTime))]
        public async Task<IHttpActionResult> GetScheduleTime(int id)
        {
            var query = from st in db.ScheduleTimes
                        where st.schedule_type == id
                        select DbFunctions.CreateTime(st.time.Hour, st.time.Minute,st.time.Minute);

            if(query == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(query);
            }
        }


    }
}