using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Data
{
    public class PermissionsDTO
    {
        public int permission_id { set; get; }

        //Foreign key for user_id
        public int user_id { set; get; }
       

        //Foreign key for role_id
        public int role_id { set; get; }
        

        //Foreign key for app_id
        public int app_id { set; get; }
      

        //permission dates
        public DateTime? date_from { get; set; }

        public DateTime? date_to { get; set; }

        public int active { get; set; }

        
        public int delegator { set; get; }
        
    }
}