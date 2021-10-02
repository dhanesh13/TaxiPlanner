using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Models
{
    public class NotificationToken
    {
        [Key]
        [Column(Order = 0)]
        public int emp_id { get; set; } //fk

        [Key]
        [Column(Order = 1)]
        [MaxLength(500)]
        public string token { get; set; }
        public string device_id { get; set; }
        public string os { get; set; }

    }
}