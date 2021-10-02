using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using TaxiPlannerAPI.Models;
using TaxiPlannerAPI.Data;
using TaxiPlannerAPI.Services;

namespace TaxiPlannerAPI.Controllers
{

    public class EmployeesController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();

        // GET: api/Employees
        [Authorizer]
        [Authorize(Roles = "hr, superadmin, approver,manager,delegated_manager")]
        [HttpGet]
        public async Task<HttpResponseMessage> Getusers()
        {
            return Request.CreateResponse(HttpStatusCode.OK, await Nucleus.GetEmployeedetails());
        }

        [HttpGet]
        [ResponseType(typeof(EmployeeDTO))]
        public async Task<HttpResponseMessage> Getuser(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, await Nucleus.GetEmployee(id));
        }
    }
}