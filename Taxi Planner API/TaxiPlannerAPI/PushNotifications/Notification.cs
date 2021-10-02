using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace TaxiPlannerAPI.PushNotifications
{
    public class Notification
    {

        public async System.Threading.Tasks.Task sendNotificationAsync(string FireBasePushNotificationsURL,string ServerKey,)
        {
            //Create request to Firebase API
            var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);
            request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application / Json");
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                result = await client.SendAsync(request);
            }
        }
        
    }
}