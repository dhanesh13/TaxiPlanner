using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI;
using TaxiPlannerAPI.Data;
using TaxiPlannerAPI.Models;
using WebGrease.Configuration;
using AutoMapper;
using System.Net.Http.Headers;
using System.Threading;
using TaxiPlannerAPI.Controllers;
using Microsoft.Ajax.Utilities;

namespace TaxiPlannerAPI.Services
{
    public class Nucleus : ApiController
    {

        static List<SessionDTO>  authorize = AuthorizationService.Instance.sessions;
      

        [Authorizer]
        public static async Task<List<EmployeeDTO>> GetEmployeedetails()
        {
            int id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

            var token = authorize.Where(s => s.EmployeeID == id).FirstOrDefault().Token;

            var uri = Appsettings.uri+"/users";
            HttpClient client  = new HttpClient();


            client.DefaultRequestHeaders.Accept.Clear();
           
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = await client.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<List<EmployeeDTO>>(content); ;
        }

        [Authorizer]
        public static async Task<List<EmployeeDTO>> GetEmployee(int id)
        {

           

            var uri = Appsettings.uri + "/users/"+id;
            HttpClient client = new HttpClient();
            

            var content = await client.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<List<EmployeeDTO>>(content);
        }

        public static async Task<List<PermissionsDTO>> GetPermission()
        {
            //int id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

            var app_id = 2;

            var uri = Appsettings.uri + "/permissions/" + app_id;
            HttpClient client = new HttpClient();



            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token " + authorize.getToken(id));

            var content = await client.GetStringAsync(uri);
            //if (content == null)
            //{
            //    app_id = 2;
            //}

            return JsonConvert.DeserializeObject<List<PermissionsDTO>>(content); ;
        }


        public static async Task<List<PermissionsDTO>> GetAccess(String token)
        {

            var app_id = 2;

            var uri = Appsettings.uri + "/permissions/"+app_id;            
            HttpClient client= new HttpClient();


        
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

          

            var content = await client.GetStringAsync(uri);
          

            return JsonConvert.DeserializeObject<List<PermissionsDTO>>(content); ;
        }

        [Authorizer]
        public static DelegatePermissionDTO DelegateRole(DelegatePermissionDTO permission)
        {

            int id = int.Parse(Thread.CurrentPrincipal.Identity.Name);

           

            var token = authorize.Where(s => s.EmployeeID == id).FirstOrDefault().Token;



            var uri = Appsettings.uri + "/permissions";
            HttpClient client = new HttpClient();



            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);




            var postContent = new StringContent(JsonConvert.SerializeObject(permission), Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, postContent).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                string res_str = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<DelegatePermissionDTO>(res_str);

                //return await Task.Run(() => EmployeeDTO.Parse(response.Content.ReadAsStringAsync().Result
            }
            else
            {
                return null;
            }
        }

    }
}
