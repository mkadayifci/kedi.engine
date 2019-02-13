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
            //activeSessions.Add(
            //                    "gh463d1\\ScheduledService.dmp",
            //                    new Session(
            //                                "gh463d1\\ScheduledService.dmp",
            //                                "30230bf96a884830a0b96805cf173717",
            //                                "ScheduledService.dmp"));
            activeSessions.Add(
                                "gh463d1\\ScheduledService_ConsoleApp.dmp",
                                new Session(
                                            "gh463d1\\ScheduledService_ConsoleApp2.dmp",
                                            "30230bf96a884830a0b96805cf173717",
                                            "ScheduledService_ConsoleApp2.dmp"));




        }



        public Session GetById(string sessionId)
        {
            foreach (Session session in activeSessions.Values)
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
                activeSessions.Add(identifier, new Session(identifier, Guid.NewGuid().ToString("N"),identifier));

            }

            return activeSessions[identifier];

        }
    }

}
