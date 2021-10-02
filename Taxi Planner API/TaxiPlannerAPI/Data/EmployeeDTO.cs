using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaxiPlannerAPI.Models;

namespace TaxiPlannerAPI.Data
{
    public class EmployeeDTO
    {
        public int user_id { set; get; }
        public String user_name { set; get; }
        public String email { set; get; }
        public String role_name { set; get; }
        //public String role {
        //    set { this.role = "hr" }
        //    get
        //    {
        //        return this.role;
        //    }
        //}

        public int role_id { set; get; }

        public String address_line { get; set; }

        public int costcenter_id { set; get; }
        public String costcenter_name { get; set; }

        public int segment_id { set; get; }
        public String segment_name { get; set; }

        public int superior_id { set; get; }
        public String superior_name { get; set; }

        public string region { set; get; }
        public string sub_regions { set; get; }
    }
}