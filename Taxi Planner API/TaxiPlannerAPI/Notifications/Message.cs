﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI.Notifications
{
    public class Message
    {
        public string[] registration_ids { get; set; }
        public Notification notification { get; set; }
        public object data { get; set; }
    }
}