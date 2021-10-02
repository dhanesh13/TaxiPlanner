using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TaxiPlannerAPI.Notifications
{
    public class PushNotification
    {

        private static Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
        //private static string ServerKey = "AAAAS8TiqL8:APA91bF-eGD7ghg6WuRpeFhJS4Ny2Rv-r9qF2PbEgD0bX5lQVT57PprQvXPdCr1Soc-teLFDGUCNgS3LIDuBuwQP-Z7L71H67mRcV6_UC3jCYHrjW4wz_a_ffZ0v0zaOvRxMfoDO_zSh";
        private static string ServerKey = System.Configuration.ConfigurationManager.AppSettings["ServerKey"];

        public static async Task<bool> SendPushNotification(string[] deviceTokens, string title, string body)
        {
            bool sent = false;

            if (deviceTokens.Count() > 0)
            {

                var messageInformation = new Message()
                {
                    notification = new Notification()
                    {
                        title = title,
                        text = body
                    },
                    registration_ids = deviceTokens
                };

                //Object to JSON STRUCTURE => using Newtonsoft.Json;
                string jsonMessage = JsonConvert.SerializeObject(messageInformation);

                //Create request to Firebase API
                var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);

                request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);
                request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                HttpResponseMessage result;
                using (var client = new HttpClient())
                {
                    result = await client.SendAsync(request);
                    sent = sent && result.IsSuccessStatusCode;
                }
            }

            return sent;
        }

    }
}