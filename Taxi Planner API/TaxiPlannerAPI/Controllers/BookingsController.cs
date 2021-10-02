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
    [RoutePrefix("api")]
    public class BookingsController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();

        // GET: api/Bookings
        [Route("bookings")]
        [HttpGet]
        public IQueryable<Booking> GetBookings()
        {
            return db.Bookings;
        }

        // GET: api/Bookings/5
        [HttpGet]
        [ResponseType(typeof(Booking))]
        public async Task<IHttpActionResult> GetBooking(int id)
        {
            Booking booking = await db.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // DELETE: api/Bookings/5
        [ResponseType(typeof(Booking))]
        public async Task<IHttpActionResult> DeleteBooking(int id)
        {
            Booking booking = await db.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            db.Bookings.Remove(booking);
            await db.SaveChangesAsync();

            return Ok(booking);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookingExists(int id)
        {
            return db.Bookings.Count(e => e.booking_id == id) > 0;
        }
    }
}