using System.IdentityModel.Tokens.Jwt;
using System.Web.Http;
using System.Web.Http.Description;
using TaxiPlannerAPI.Models;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Results;
using System.Linq;
using TaxiPlannerAPI.Notifications;
using System.Collections.Generic;
using TaxiPlannerAPI.Services;
using TaxiPlannerAPI.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Net.Http;
using System.Net;

namespace TaxiPlannerAPI.Controllers
{

    [Route("api/auth")]
    public class AuthsController : ApiController
    {
        private TaxiPlannerAPIContext db;
        private AuthorizationService auth;
        private List<PermissionsDTO> t;

        public AuthsController()
        {
            db = new TaxiPlannerAPIContext();
            auth = AuthorizationService.Instance;
            
        }

        //// GET: api/auth
        //[Authorizer]
        //[Authorize]
        //[HttpGet]
        //public OkNegotiatedContentResult<string> Test()
        //{
        //    System.Diagnostics.Debug.WriteLine("Name: " + Thread.CurrentPrincipal.Identity.Name);
        //    var role = Thread.CurrentPrincipal.IsInRole("hr") ? "hr" : Thread.CurrentPrincipal.IsInRole("manager") ? "manager" : "employee";
        //    return Ok("ID: " + Thread.CurrentPrincipal.Identity.Name + " " + "Role: " + role);
        //}

        //// POST: api/auth
        //[ResponseType(typeof(AuthRes))]
        //[HttpPost]
        //public async Task<IHttpActionResult> Login(Auth authReq)
        //{
        //    EmployeeDTO employee = Nucleus.Login(authReq);

        //    //RoleDTO role = await Nucleus.GetRolename(employee.user_id);

        //    if (employee != null)
        //    {
        //        try
        //        {
        //            NotificationToken checktoken = await GetTokens(authReq);
        //            if (checktoken == null && authReq.device_id != null && authReq.token != null && authReq.os != null)
        //            {
        //                NotificationToken n = new NotificationToken();
        //                n.emp_id = employee.user_id;
        //                n.token = authReq.token;
        //                n.device_id = authReq.device_id;
        //                n.os = authReq.os;
        //                db.NotificationTokens.Add(n);
        //                await db.SaveChangesAsync();

        //            }
        //        }

        //        catch (SqlException sqlEx)
        //        {
        //        }
        //        catch (Exception ex)
        //        {
        //        }

        //        var permissions = await Nucleus.GetPermission();
        //        var rolename = (from p in permissions
        //                        join r in db.Roles
        //                        on p.role_id equals r.role_id
        //                        where p.user_id == employee.user_id
        //                        && (p.date_to == null || (p.date_from < DateTime.Now && p.date_to > DateTime.Now))
        //                        orderby p.role_id descending
        //                        select r.name).FirstOrDefault();

        //        //the query will only display expirydates which are null, meaning the original role of the user
        //        // or if there is a datetime, it checks if that time has already expired
        //        //it then returns the highest position. 
        //        //assume that roleid of delegated is the highest position

        //        var claims = new[]
        //        {
        //            // TODO: save keys in external file
        //            new Claim(JwtRegisteredClaimNames.Sub, "Subject"),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        //            new Claim("name", employee.user_name),
        //            new Claim("role", rolename),
        //            new Claim("id", employee.user_id.ToString())
        //        };

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Appsettings.Secret));

        //        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken("Issuer", "Audience", claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

        //        AuthRes res = new AuthRes();
        //        res.token = new JwtSecurityTokenHandler().WriteToken(token);
        //        res.role = rolename;
        //        res.name = employee.user_name;
        //        res.id = employee.user_id;


        //        System.Diagnostics.Debug.WriteLine("login role: " + res.role);
        //        IPrincipal principal = new GenericPrincipal(new GenericIdentity(employee.user_id.ToString()), new string[] { rolename });
        //        Thread.CurrentPrincipal = principal;
        //        if (HttpContext.Current != null)
        //        {
        //            HttpContext.Current.User = principal;
        //        }

        //        return Ok(res);
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid Credentials");
        //    }
        //}


        //Notifications: Tavish
        //private async Task<NotificationToken> GetTokens(Auth auth)
        //{
        //    return await db.NotificationTokens.FirstOrDefaultAsync(e => e.emp_id == auth.emp_id && e.token == auth.token);
        //}

        [HttpGet]
        public IHttpActionResult SessionDetails(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return Unauthorized();

                var stream = token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

                SessionDTO decryptJwt = new SessionDTO()
                {
                    Role = tokenS.Claims.First(claim => claim.Type == "role").Value,
                    Token = token, 
                    EmployeeName = tokenS.Claims.First(claim => claim.Type == "name").Value,
                    EmployeeID = Convert.ToInt32(tokenS.Claims.First(claim => claim.Type == "id").Value),
                    active = true,
                };

                auth.AddSession(decryptJwt);
                return Ok(decryptJwt);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> NucleusLogin(LoginDTO login)
        {
            try
            {
                List<PermissionsDTO> permissions = await Nucleus.GetAccess(login.token);

                var rolename = (from p in permissions
                                join r in db.Roles
                                on p.role_id equals r.role_id
                                where p.user_id == login.id && p.app_id == 2
                                && (p.date_to == null || p.date_to > DateTime.Now)
                                orderby p.role_id descending
                                select r.name).FirstOrDefault();


                //var rolename = (from ro in db.Roles
                //                where ro.role_id == login.role_id
                //                select ro.name).FirstOrDefault();

                // map login to session
                SessionDTO session = new SessionDTO();
                session.EmployeeID = login.id;
                session.EmployeeName = login.name;
                session.Role = rolename;
                session.Token = login.token;

                // store 
                auth.AddSession(session);

                return Ok(session);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
           
        }

        //public async Task<HttpResponseMessage> GetRole(string token) {





        //    return Request.CreateResponse(HttpStatusCode.OK, );

        //}



        //    [ResponseType(typeof(NotificationToken))]
        //public async Task<IHttpActionResult> DeleteToken(int emp_id, string token1)
        //{
        //    NotificationToken test = await db.NotificationTokens.FirstOrDefaultAsync(e => e.emp_id == emp_id && e.token == token1);
        //    if (test == null)
        //    {
        //        return NotFound();
        //    }

        //    db.NotificationTokens.Remove(test);
        //    await db.SaveChangesAsync();

        //    return Ok(test);
        //}
    }
}