using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TaxiPlannerAPI.Data;
using TaxiPlannerAPI.Services;

namespace TaxiPlannerAPI.Controllers
{
    public class Authorizer : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
            {
            try
            {
                if (actionContext.Request.Headers == null || actionContext.Request.Headers.Authorization == null)
                    throw new Exception();

                // get token from the header of the incoming request
                var token = actionContext.Request.Headers.Authorization.Parameter;

                AuthorizationService authorization = AuthorizationService.Instance;

                SessionDTO session = authorization.ValidateToken(token);
                // validate using Authorization service
                if(session != null)
                {

                    string role = session.Role;
                    string id = session.EmployeeID.ToString();

                    System.Diagnostics.Debug.WriteLine(id + " auth as: " + role);
                    IPrincipal principal = new GenericPrincipal(new GenericIdentity(id), new string[] { role });
                    SetPrincipal(principal);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
        public void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

    }
}
