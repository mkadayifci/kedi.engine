using kedi.engine.Services.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    
    public class SessionController : ApiController
    {
        ISessionManager sessionManager = ContainerManager.Container.Resolve<ISessionManager>();

        [HttpGet]
        public Session[] Get()
        {
            return sessionManager.GetSessions().Values.ToArray();
        }
        [HttpPost]

        public dynamic Post([FromBody]dynamic sessionStartInfo)
        {
           return sessionManager.Add(sessionStartInfo.identifier.ToString());
        }
    }
}