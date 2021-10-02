using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TaxiPlannerAPI.Models;

namespace TaxiPlannerAPI.Controllers
{
    public class TokensController : ApiController
    {

        // GET: api/Tokens
        // used to check if user is logged in. returns the logged in user
        public string GetToken()
        {
            return "testing";
        }

        // POST: api/Tokens
        // used to authenticate a user
        //[ResponseType(typeof(Token))]
        //public async Task<IHttpActionResult> PostToken(AuthRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = token.token }, token);
        //}
    }
}