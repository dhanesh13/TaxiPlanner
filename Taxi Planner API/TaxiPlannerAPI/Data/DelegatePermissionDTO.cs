using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Data
{
    public class DelegatePermissionDTO
    {
        /// Only these fields need to be supplied
        [Required]
        public int user_id { set; get; }
        [Required]
        public DateTime? date_from { get; set; }
        [Required]
        public DateTime? date_to { get; set; }

        /// 
        public int delegator { set; get; }
        public int app_id { set; get; } = 1;

        public int role_id { set; get; }


    }
}