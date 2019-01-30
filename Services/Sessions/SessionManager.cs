using System;
using System.Collections.Generic;

namespace kedi.engine.Services.Sessions
{
    public class SessionManager : ISessionManager
    {
        private Dictionary<string, Session> activeSessions = new Dictionary<string, Session>();
        public Dictionary<string, Session> GetSessions()
        {
            return activeSessions;
        }
        public SessionManager()
        {
            activeSessions.Add("gh463d1\\ScheduledService.dmp", new Session()
            {
                SessionId = "30230bf96a884830a0b96805cf173717",
                Identifier = "gh463d1\\ScheduledService.dmp",
                SourceName= "ScheduledService.dmp"
            });
       
        }


        
        public Session GetById(string sessionId)
        {
            foreach(Session session in activeSessions.Values)
            {
                if (session.SessionId == sessionId)
                    return session;
            }
            return null;
   
        }

        public Session Add(string identifier)
        {
            if (!activeSessions.ContainsKey(identifier))
            {
                activeSessions.Add(identifier, new Session()
                {
                    SessionId = Guid.NewGuid().ToString("N"),
                    Identifier= identifier
                });

            }

            return activeSessions[identifier];

        }
    }
   
}
