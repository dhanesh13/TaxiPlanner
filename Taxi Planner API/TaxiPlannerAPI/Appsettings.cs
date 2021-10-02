using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiPlannerAPI
{
    public class Appsettings
    {
        public static string Secret = "dkbcndkjfnewkjfnewfneflknew";
        public static string mailFrom = "vivektavish@gmail.com";
        // using local host
        //public static string uri = "https://localhost:44390/api";
        //using local iis
        //public static string uri = "http://localhost/nucleuSD/api";
        public static string uri = "https://nucleusapi.azurewebsites.net/api";
    }
}
