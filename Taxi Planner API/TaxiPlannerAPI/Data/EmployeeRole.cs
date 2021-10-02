using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TaxiPlannerAPI.Data;

namespace TaxiPlannerAPI.Models
{
    public class EmployeeRole
    {
        [Key]
        public int emprole_id { get; set; }

        //foreign key Employee
        public int emp_id { get; set; }
        public EmployeeDTO Employees { get; set; }

        //foreign key Role
        public int role_id { get; set; }
        public Role Roles { get; set; }


        public DateTime? start_date { get; set; }
        public DateTime? expiry_date { get; set; }

        //public int? dept_id { get; set; }

        public int? delegator_id { get; set; }

    }
}
