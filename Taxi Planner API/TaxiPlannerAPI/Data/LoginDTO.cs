using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Data
{
    public class LoginDTO
    {
        public int id { set; get; }
        public String name { set; get; }
        public String token { set; get; }
        public String role { set; get; }
        public int role_id { set; get; }
    }
}