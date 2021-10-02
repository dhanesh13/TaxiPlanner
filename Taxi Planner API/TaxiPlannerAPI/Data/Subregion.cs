using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Data
{
    public class Subregion
    {
        public string region { set; get; }
        public string sub_regions { set; get; }
        public int count { set; get; } = 0;
    }
}