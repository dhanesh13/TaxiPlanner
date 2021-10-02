using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TaxiPlannerAPI.EmailServer;
using TaxiPlannerAPI.Models;
using TaxiPlannerAPI.Notifications;
using TaxiPlannerAPI.Validation;
using TaxiPlannerAPI.Data;
using TaxiPlannerAPI.Services;

namespace TaxiPlannerAPI.Controllers
{

    public class BookingSlotsController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();
        string text, title;

        [Authorizer]
        [Authorize]
        public async Task<HttpResponseMessage> GetAllBookingSlots(int choice)
        {
            if (Thread.CurrentPrincipal.IsInRole("hr") || Thread.CurrentPrincipal.IsInRole("superadmin"))
            {
                if (choice == 1)
                {
                    var users = await Nucleus.GetEmployeedetails();

                    var BookingSlots = (from bs in db.BookingSlots
                                        join b in db.Bookings
                                        on bs.booking_id equals b.booking_id
                                        orderby bs.date_time ascending
                                        where bs.date_time >= DateTime.Now
                                        select new
                                        {
                                            b.emp_id,
                                            bs.booking_id,
                                            bs.date_time,
                                            bs.reason,
                                            bs.status,
                                        }).ToList();


                    var results = (from b in BookingSlots
                                   join u in users on new { first = b.emp_id } equals new { first = u.user_id }
                                   select new
                                   {
                                       user_id = b.emp_id,
                                       name = u.user_name,
                                       b.booking_id,
                                       b.date_time,
                                       b.reason,
                                       b.status,
                                       //emp_id = u.user_id, //changed rr
                                       u.costcenter_name,
                                       u.segment_name,
                                   }).ToList();

                  
                        return Request.CreateResponse(HttpStatusCode.OK, results);
                    
                }

                else if (choice == 2) //view only his bookings
                {
                    int id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

                    var users = await Nucleus.GetEmployee(id);

                    var BookingSlots = (from bs in db.BookingSlots
                                        join b in db.Bookings
                                        on bs.booking_id equals b.booking_id
                                        orderby bs.date_time ascending
                                        where bs.date_time >= DateTime.Now
                                        select new
                                        {
                                            b.emp_id,
                                            bs.booking_id,
                                            bs.date_time,
                                            bs.reason,
                                            bs.status,
                                        }).ToList();


                    var results = (from b in BookingSlots
                                   join u in users on new { first = b.emp_id } equals new { first = u.user_id }
                                   select new
                                   {
                                       id = b.emp_id,
                                       name = u.user_name,
                                       b.booking_id,
                                       b.date_time,
                                       b.reason,
                                       b.status,
                                       //u.user_id,
                                       u.costcenter_name,
                                       u.segment_name,
                                   }).ToList();

                  
                        return Request.CreateResponse(HttpStatusCode.OK, results);
                    
                }

                else
                {

                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This request does not exist");

                }
            }

            if (Thread.CurrentPrincipal.IsInRole("employee"))
            {

                //does not matter which choice
                int id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

                var users = await Nucleus.GetEmployee(id);

                var BookingSlots = (from bs in db.BookingSlots
                                    join b in db.Bookings
                                    on bs.booking_id equals b.booking_id
                                    orderby bs.date_time ascending
                                    where bs.date_time >= DateTime.Now
                                    select new
                                    {
                                        b.emp_id,
                                        bs.booking_id,
                                        bs.date_time,
                                        bs.reason,
                                        bs.status,
                                    }).ToList();

                var results = (from b in BookingSlots
                               join u in users on new { first = b.emp_id } equals new { first = u.user_id }
                               //into bujoin
                               //from bu in bujoin.DefaultIfEmpty()
                               select new
                               {
                                   id = b.emp_id,
                                   name = u.user_name,
                                   b.booking_id,
                                   b.date_time,
                                   b.reason,
                                   b.status,
                                   u.user_id,
                                   u.costcenter_name,
                                   u.segment_name,
                               }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, results);
            }
            else
            {
                int approver_id = 0; //approver_id is the id of the one having the permission to approve.
                                     //if delegated, the person gets the same approver_id of the delegator
                                     //and is able to perform all his tasks. FrontEnd will limit his views though

                if (Thread.CurrentPrincipal.IsInRole("approver"))
                {
                    approver_id = int.Parse(Thread.CurrentPrincipal.Identity.Name);
                }
                if (Thread.CurrentPrincipal.IsInRole("delegated_approver"))
                {
                    int id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

                    var delegated = await Nucleus.GetEmployee(id);

                    approver_id = (int)(from d in delegated
                                        select d.superior_id).FirstOrDefault();

                   
                }
                if (choice == 1) //view all bookings
                {
                    var users = await Nucleus.GetEmployeedetails();

                    var manager = await Nucleus.GetEmployee(approver_id);

                    var BookingSlots = (from bs in db.BookingSlots
                                        join b in db.Bookings
                                        on bs.booking_id equals b.booking_id
                                        orderby bs.date_time ascending
                                        where bs.date_time >= DateTime.Now
                                        select new
                                        {
                                            b.emp_id,
                                            bs.booking_id,
                                            bs.date_time,
                                            bs.reason,
                                            bs.status,
                                        }).ToList();

                    var results = (from b in BookingSlots
                                   join u in users on new { first = b.emp_id } equals new { first = u.user_id }
                                   join us in manager
                                   on u.superior_id equals us.user_id
                                   where us.user_id == approver_id
                                   && b.date_time >= DateTime.Now
                                   orderby b.date_time ascending
                                   select new
                                   {
                                       id = b.emp_id,
                                       name = u.user_name,
                                       b.booking_id,
                                       b.date_time,
                                       b.reason,
                                       b.status,
                                       u.user_id,
                                       u.costcenter_name,
                                       u.segment_name,
                                   }).ToList();

                    //if (results.Count == 0)
                    //{
                    //    return Request.CreateResponse(HttpStatusCode.BadRequest, "No bookings in the future");
                    //}
                    //else
                    //{
                        return Request.CreateResponse(HttpStatusCode.OK, results);
                    
                }
                else if (choice == 2) //view only his bookings
                {
                    int id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

                    var users = await Nucleus.GetEmployee(id);

                    var BookingSlots = (from bs in db.BookingSlots
                                        join b in db.Bookings
                                        on bs.booking_id equals b.booking_id
                                        orderby bs.date_time ascending
                                        where bs.date_time >= DateTime.Now
                                        select new
                                        {
                                            b.emp_id,
                                            bs.booking_id,
                                            bs.date_time,
                                            bs.reason,
                                            bs.status,
                                        }).ToList();

                    var results = (from b in BookingSlots
                                   join u in users on new { first = b.emp_id } equals new { first = u.user_id }
                                   where u.user_id == id
                                   && b.date_time >= DateTime.Now
                                   //sees only your bookings, not as a delegator.
                                   orderby b.date_time ascending
                                   select new
                                   {
                                       id = b.emp_id,
                                       name = u.user_name,
                                       b.booking_id,
                                       b.date_time,
                                       b.reason,
                                       b.status,
                                       u.user_id,
                                       u.costcenter_name,
                                       u.segment_name,
                                   }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, results);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This request does not exist");
                }
            }
        }

        [Authorizer]
        [Authorize]
        [HttpPost]
        public async Task<HttpResponseMessage> PostBookingSlot([FromBody] List<BookingSlot> bookingslots)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var users = await Nucleus.GetEmployeedetails();

                    Booking newbooking = new Booking();
                    int emp_id = bookingslots[0].emp_id;
                    //assuming the whole bookings made by the approver belongs to one person only
                    //So the emp_id of the first booking slot will be the same as all the others
                    //finding the email and name of the person whom this employer reports to direcly

                    List<EmployeeDTO> approver = (from u in users
                                    join us in users
                                    on u.superior_id equals us.user_id
                                    where u.user_id == emp_id
                                    select us).ToList(); //

                    var employee = await Nucleus.GetEmployee(emp_id); 

                    var TheToken = (from tk in db.NotificationTokens
                                    where tk.emp_id == emp_id //changed
                                    select tk.token).ToArray();

                    var thetoken1 = (from tk in db.NotificationTokens
                                     where tk.emp_id == emp_id //changed
                                     select tk.token).ToArray();

                    if (approver == null)
                    {
                       approver = await Nucleus.GetEmployee(emp_id);
                       
                    }


                    newbooking.emp_id = emp_id;
                    newbooking.timestamp_booked = DateTime.Now;
                    db.Bookings.Add(newbooking);
                    db.SaveChanges();
                    int id = newbooking.booking_id;
                    var bookingdatetimes = "\n"; //to store the list of booking datetimes

                    if (emp_id == int.Parse(Thread.CurrentPrincipal.Identity.Name)) //the person is making a booking for himself
                    {
                        foreach (BookingSlot slot in bookingslots)
                        {
                            slot.booking_id = id;
                            slot.status = BookingStatus.PENDING;

                            if (Validation.Validate.ValidateBooking(slot.date_time, emp_id, db))
                            {
                                transaction.Rollback();
                                return Request.CreateResponse(HttpStatusCode.Forbidden, "Duplicate booking slots");
                            };

                            db.BookingSlots.Add(slot);
                            bookingdatetimes += slot.date_time.ToString("dddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US")) + " " + slot.date_time.ToString("hh:mm tt", CultureInfo.InvariantCulture) + "\n Reason: " + slot.reason + "\n";
                            bookingdatetimes += "\n";
                        }

                        EmailActions.SendEmailBookingByEmployee(employee[0].user_name, employee[0].email, employee[0].address_line, approver[0].user_name, approver[0].email, bookingdatetimes);
                    }
                    else //the person is making a booking for his subordinates
                    {
                        //to get the name of the person making the booking
                        int logger_id = int.Parse(Thread.CurrentPrincipal.Identity.Name);
                        var logger_name = (from u in users
                                           where u.user_id == logger_id
                                           select u.user_name).FirstOrDefault();

                        foreach (BookingSlot slot in bookingslots)
                        {
                            slot.booking_id = id;
                            slot.status = BookingStatus.APPROVED;
                            if (Validation.Validate.ValidateBooking(slot.date_time, emp_id, db))
                            {
                                transaction.Rollback();
                                return Request.CreateResponse(HttpStatusCode.Forbidden, "Duplicate booking slots");
                            };
                            db.BookingSlots.Add(slot);
                            bookingdatetimes += slot.date_time.ToString("dddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US")) + " " + slot.date_time.ToString("hh:mm tt", CultureInfo.InvariantCulture) + "\n Reason: " + slot.reason + "\n";
                            bookingdatetimes += "\n";
                        }
                     
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Multiple BookingSlots cannot have same datetime" + e); //e.g duplicate primary key that is same datetime
                }
            }
        }

        [Authorizer]
        [Authorize(Roles = "approver, delegated_approver, superadmin")]
        [HttpPatch]
        public async Task<HttpResponseMessage> PartiallyUpdateStatus([FromBody] List<Status> new_state_list)
        {
            var users = await Nucleus.GetEmployeedetails();
            //getting the details of the person who will update the booking
            int approver_id = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            var approver_email = (from u in users
                                  where u.user_id == approver_id
                                  select u.email).FirstOrDefault();
            try
            {
                foreach (Status new_state in new_state_list)
                {
                    string time1 = "";
                    string date1 = "";
                    BookingSlot slot = await db.BookingSlots.SingleOrDefaultAsync(x => x.booking_id == new_state.booking_id && x.date_time == new_state.bookingslot_date_time);

                    var booked = (from bs in db.BookingSlots
                                  join b in db.Bookings
                                  on bs.booking_id equals b.booking_id

                                  where bs.booking_id == new_state.booking_id && bs.date_time == new_state.bookingslot_date_time
                                  select new { b.emp_id }).ToList();

                    var employee = (from b in booked
                                    join u in users on new { first = b.emp_id } equals new { first = u.user_id }
                                    select new
                                    {
                                        name = u.user_name,
                                        u.email,
                                        emp_id = u.user_id
                                    }).FirstOrDefault();

                    if (slot == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Booking Slot does not exist");
                    }

                    BookingStatus status;
                    Enum.TryParse<BookingStatus>(new_state.status, out status);
                    slot.status = status;
                    slot.evaluation_timestamp = DateTime.Now;
                    slot.approved_by_id = approver_id;
                    if (new_state.updated_date_time == default(DateTime))   //changing only the status
                    {
                        db.Entry(slot).State = EntityState.Modified;
                        date1 = new_state.bookingslot_date_time.ToString("dddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                        time1 = new_state.bookingslot_date_time.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        await db.SaveChangesAsync();
                    }
                    else  //changing the date
                    {
                        BookingSlot temp = new BookingSlot();
                        temp.booking_id = slot.booking_id;
                        temp.date_time = new_state.updated_date_time;
                        temp.status = BookingStatus.APPROVED;
                        temp.reason = slot.reason;
                        temp.evaluation_timestamp = slot.evaluation_timestamp;
                        temp.approved_by_id = approver_id;
                        db.BookingSlots.Remove(slot);
                        await db.SaveChangesAsync();
                        db.BookingSlots.Add(temp);
                        await db.SaveChangesAsync();
                        time1 = new_state.updated_date_time.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        date1 = new_state.updated_date_time.ToString("dddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                    //finding the name of the approver
                    var name = (from u in users
                                where u.user_id == approver_id
                                select u.user_name).FirstOrDefault();

                    //finding token
                    var Thetoken = (from tk in db.NotificationTokens
                                    where tk.emp_id == employee.emp_id
                                    select tk.token).ToArray();


                    if (name == null) //to avoid breaks if the database does not contain an approver
                    {
                        name = "";
                    }

                    if (slot.status == BookingStatus.APPROVED)
                    {
                        if (Thetoken != null)
                        {
                            text = "Date:" + date1 + "\n" + "Time:" + time1;
                            title = "Booking Approved";
                            //await PushNotification.SendPushNotification(Thetoken, title, text);
                        }

                        EmailActions.SendEmailStatusChanged(employee.name, employee.email, slot.status, date1, time1, name);

                    }
                    else if (slot.status == BookingStatus.REJECTED)
                    {
                        if (Thetoken != null)
                        {
                            text = "Date:" + date1 + "\n" + "Time:" + time1;
                            title = "Booking Rejected";
                            //await PushNotification.SendPushNotification(Thetoken, title, text);
                        }
                        EmailActions.SendEmailStatusChanged(employee.name, employee.email, slot.status, date1, time1, name);

                    }
                    else if (new_state.updated_date_time != default)
                    {
                        if (Thetoken != null)
                        {
                            text = "Date:" + date1 + "\n" + "Time:" + time1;
                            title = "Booking Approved with a change in time";
                            //await PushNotification.SendPushNotification(Thetoken, title, text);
                        }
                        EmailActions.SendEmailTimeChanged(employee.name, employee.email, date1, time1, name);

                    }
                } //end of foreach
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Update successful");
        }

    }
}
