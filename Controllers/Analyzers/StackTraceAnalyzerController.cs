using kedi.engine.Services.Analyzers;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class StackTraceAnalyzerController : ApiController
    {
        StackTraceAnalyzer analyzer = new StackTraceAnalyzer();

        [Route("api/analyzers/stack-trace-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }
    }

}



