using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class AuthRes
    {
        public string token { set; get; }
        public string name { set; get; }
        public string role { set; get; }
        public int id { set; get; }
    }
}