using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class Region
    {
        [Key]
        public string region_name { get; set; }

        [Required]
        public float estimated_price { get; set; }
    }
}