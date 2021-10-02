using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Data
{
    public class CostcenterDTO
    {

        public int costcenter_id { set; get; }
       
        public string costcenter_name { get; set; }

        public int segment_id { set; get; }
       
    }

}