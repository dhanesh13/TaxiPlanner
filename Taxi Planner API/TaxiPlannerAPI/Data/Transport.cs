using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Data
{
    public class Transport
    {
        public string region { set; get; }

        public DateTime timestamp { set; get; }

        // min number of passengers required to allocate another transport for subregion
        //public int threshold { set; get; }

        // number of passengers which can be accommodated in the transport
        //public int seats { set; get; }

        // some transport may drop people at multiple subregions
        public HashSet<string> subregions { set; get; } = new HashSet<string>();

        // the number of passenger to allocate to this transport
        public int passengers_count { set; get; } = 0;

        // employees allocated to this transport
        public List<EmployeeDTO> passengers { set; get; } = new List<EmployeeDTO>();
    }
}