using kedi.engine.Services.Sessions;
using System.Linq;
using System.Web.Http;

namespace kedi.engine.Controllers
{

    public class SessionController : ApiController
    {
        ISessionManager sessionManager = ContainerManager.Container.Resolve<ISessionManager>();

        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(sessionManager.GetSessions().Values.ToArray());
        }
        [HttpPost]

        public IHttpActionResult Post([FromBody]dynamic sessionStartInfo)
        {
            return Ok(sessionManager.Add(sessionStartInfo.identifier.ToString()));
        }
    }
}