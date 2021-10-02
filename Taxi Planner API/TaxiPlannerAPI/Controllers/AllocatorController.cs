using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using TaxiPlannerAPI.Data;
using TaxiPlannerAPI.Models;
using TaxiPlannerAPI.Services;

namespace TaxiPlannerAPI.Controllers
{
    public class AllocatorController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();
        private ReportService rs = new ReportService();

        [HttpGet]
        public async Task<IHttpActionResult> AllocateTransport()
        {
            List<Transport> transports = await AllocationController.GenerateAll();
            rs.GenerateReport1(transports);
            rs.GenerateReport2(transports);

            return Ok(await AllocationController.GenerateAll());
        }

        [HttpPost]
        public IHttpActionResult ReviewedList(List<Transport> transports)
        {
            // takes the reviewed list and generates the report
            rs.GenerateReport1(transports);
            rs.GenerateReport2(transports);

            return Ok(transports);
        }
    }
}
