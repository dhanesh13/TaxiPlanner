using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiPlannerAPI.Models;

namespace TaxiPlannerAPI.Controllers
{
    public class TokenController : ApiController
    {
        private TaxiPlannerAPIContext db = new TaxiPlannerAPIContext();
        public IConfiguration _configuration;
    }
}
