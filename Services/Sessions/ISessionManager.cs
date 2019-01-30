using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Sessions
{
    public interface ISessionManager
    {
        Dictionary<string, Session> GetSessions();
        Session Add(string identifier);
        Session GetById(string sessionId);
    }
}
