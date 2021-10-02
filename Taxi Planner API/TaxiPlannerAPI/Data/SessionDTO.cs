using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Data
{
    public class SessionDTO
    {
        public string Token { set; get; }
        public int EmployeeID { set; get; }
        public string EmployeeName { set; get; }
        public string Role { set; get; }
        public DateTime validTill { set; get; }
        public bool active { set; get; } = true;
    }
}