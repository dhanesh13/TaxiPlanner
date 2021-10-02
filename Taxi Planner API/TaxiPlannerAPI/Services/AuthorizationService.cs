using System.Collections.Generic;
using System.Linq;
using TaxiPlannerAPI.Data;

namespace TaxiPlannerAPI.Services
{
    public sealed class AuthorizationService
    {
        public List<SessionDTO> sessions;
        public static AuthorizationService instance = null;

        private AuthorizationService()
        {
            sessions = new List<SessionDTO>();
        }

        public static AuthorizationService Instance
        {
            get
            {
                // instantiate
                if (instance == null)
                    instance = new AuthorizationService();

                return instance;
            }

           


        }

        public string getToken(int userId)
        {
            return sessions.Where(s => s.EmployeeID == userId).FirstOrDefault().Token;

        }

        public void AddSession(SessionDTO newSession)
        {
            // initialize
            if (sessions == null)
                sessions = new List<SessionDTO>();

            sessions.Add(newSession);
        }

        public SessionDTO ValidateToken(string token)
        {
            var session = sessions.Where(s => s.Token == token && s.active).FirstOrDefault();
            return session;
        }

       

        public void InvalidateSession(string Token)
        {
        }
    }
}