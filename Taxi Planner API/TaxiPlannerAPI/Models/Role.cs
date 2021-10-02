using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class Role
    {
        [Key]
        public int role_id { set; get; }
        [Required]
        public string name { get; set; }

    }
}