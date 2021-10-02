using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TaxiPlannerAPI.EmailServer;
using TaxiPlannerAPI.Models;
using TaxiPlannerAPI.Notifications;
using TaxiPlannerAPI.Data;
using TaxiPlannerAPI.Services;
namespace TaxiPlannerAPI.Controllers
{
    public class DelegationController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();
        string text;
        string title;

        // POST: api/Delegation
        [Authorizer]
        [Authorize(Roles = "approver, superadmin, delegated_approver")]
        [ResponseType(typeof(EmployeeRole))]
        public async Task<IHttpActionResult> PostEmployeeRole(DelegatePermissionDTO permission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await Nucleus.GetEmployee(permission.user_id);

            var delegator = await Nucleus.GetEmployee(permission.delegator);


            // EmployeeRole temp = new EmployeeRole();
            // temp.emp_id = employeeRole.emp_id;
            // temp.start_date = employeeRole.start_date;
            // temp.expiry_date = employeeRole.expiry_date;
            // temp.delegator_id = employeeRole.delegator_id;
            // temp.role_id = (from r in db.Roles
            //                where r.name == "delegated_approver"
            //                select r.role_id).FirstOrDefault();



            // set delegator id to that of the logged in user
            permission.delegator = int.Parse(Thread.CurrentPrincipal.Identity.Name);

            //permission.role_id = from r in db.Roles
            //                 where r.role_id == delegator[0].role_id
            //              select r.role_id;

            
           


            //int approver_id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

            var approver = await Nucleus.GetEmployee(permission.delegator);

            // var employee = (from emp in db.Employees
            //                 where emp.emp_id == employeeRole.emp_id
            //                 select new { emp.emp_id, emp.name, emp.email}).FirstOrDefault();

            var thetoken1 = (from tk in db.NotificationTokens
                             where tk.emp_id == permission.user_id
                             select tk.token).ToArray();

            var start_date = permission.date_from?.ToString("dddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            string expiry_date = permission.date_to?.ToString("dddd dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));

            try
            {
                //await db.SaveChangesAsync();
                EmailActions.SendMailDelegation(user[0].user_name, user[0].email, approver[0].user_name, start_date, expiry_date);
                if (thetoken1 != null)
                {
                    title = "Task Delegation by-" + approver[0].user_name;
                    text = "You have been delegated the task approver." + "\n" + "Find details below:" + "\n" + "Start Date:" + start_date + "\n" + "End Date:" + expiry_date;
                    PushNotification.SendPushNotification(thetoken1, title, text);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            // catch (DbUpdateException)
            // {
            //     if (EmployeeRoleExists(employeeRole.emp_id))
            //     {
            //         return Conflict();
            //     }
            //     else
            //     {
            //         throw;
            //     }
            // }

            // save on nucleus
            DelegatePermissionDTO permissionDTO = Nucleus.DelegateRole(permission);
            return CreatedAtRoute("DefaultApi", new { id = permissionDTO.user_id }, permissionDTO);
        }
    }
}