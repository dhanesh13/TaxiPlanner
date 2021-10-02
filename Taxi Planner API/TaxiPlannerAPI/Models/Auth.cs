using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class Auth
    {
        [Key]
        [Required]
        public string email { set; get; }
        [Required]
        public string password { set; get; }
        public string token { set; get; }//mobile token(FCM Token)

        public int emp_id { set; get; }

        public string device_id { set; get; }//For mobile only

        public string os { set; get; }//For mobile only
    }
}