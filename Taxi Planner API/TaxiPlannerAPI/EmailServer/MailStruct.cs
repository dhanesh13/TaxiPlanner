using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TaxiPlannerAPI.EmailServer
{
    public class MailStruct
    {
        [JsonProperty("approver_name")]
        public string approver_name { get; set; }

        [JsonProperty("emp_name")]
        public string emp_name { get; set; }

        [JsonProperty("emp_mail")]
        public string emp_mail { get; set; }

        [JsonProperty("emp_address")]
        public string emp_address { get; set; }

        [JsonProperty("bookingdatetime")]
        public string bookingdatetime { get; set; }

        [JsonProperty("start_from")]
        public string start_from { get; set; }

        [JsonProperty("expiry_date")]
        public string expiry_date { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("time")]
        public string time { get; set; }

        [JsonProperty("approved_by")]
        public string approved_by { get; set; }
    }
}
