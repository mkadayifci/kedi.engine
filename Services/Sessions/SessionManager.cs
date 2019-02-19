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
            //activeSessions.Add(
            //                    "gh463d1\\ScheduledService_ConsoleApp.dmp",
            //                    new Session(
            //                                "gh463d1\\ScheduledService_ConsoleApp2.dmp",
            //                                "30230bf96a884830a0b96805cf173717",
            //                                "ScheduledService_ConsoleApp2.dmp"));




        }



        public Session GetById(string sessionId)
        {
            foreach (Session session in activeSessions.Values)
            {
                if (session.SessionId == sessionId)
                    return session;
            }

            throw new SessionNotFoundException("There is no active session");

        }

        public Session Add(string path)
        {
            if (activeSessions.Values.Count > 0) { throw new Exception("There is already open session in memory, please first close it."); }

            string sessionIdForPath = this.GetSessionId(path);
            if (!activeSessions.ContainsKey(path))
            {
                activeSessions.Add(sessionIdForPath, new Session(path, sessionIdForPath, path));
            }
            return activeSessions[sessionIdForPath];
        }
        public void CloseAllSessions()
        {
            //TODO: Dispose all
            activeSessions.Clear();
        }
        public void Close(string sessionId)
        {
            //TODO:Dispose
            activeSessions.Remove(sessionId);
        }
        private string GetSessionId(string path)
        {
            System.IO.FileInfo f = new System.IO.FileInfo(path);
            return $"{path}-{f.Length}".GetHashCode().ToString().Replace("-", "X");
        }
        private void StartSession() { }



    }

}
