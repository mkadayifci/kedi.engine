using kedi.engine.Services.Analyze;
using kedi.engine.Services.Sessions;
using Microsoft.Diagnostics.Runtime;
using System.Linq;
using System.Web.Http;

namespace kedi.engine.Controllers
{

    public class SessionController : ApiController
    {
        ISessionManager sessionManager = ContainerManager.Container.Resolve<ISessionManager>();
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [Route("api/sessions")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(sessionManager.GetSessions().Values.ToArray());
        }

        [Route("api/session")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]dynamic sessionRequest)
        {
            Session addedSession = sessionManager.Add(sessionRequest.path.ToString());
            try
            {
                analyzeOrchestrator.InitRuntimeBySessionId(addedSession.SessionId);
            }
            catch (ClrDiagnosticsException ex)
            {
                sessionManager.Close(addedSession.SessionId);
                
                return BadRequest(ex.Message);
            }
            return Ok(addedSession.SessionId);
        }

        [Route("api/session/close-all")]
        [HttpPost]
        public void CloseAllSessions()
        {
            analyzeOrchestrator.RemoveAllRuntimes();
            sessionManager.CloseAllSessions();
        }

    }
}