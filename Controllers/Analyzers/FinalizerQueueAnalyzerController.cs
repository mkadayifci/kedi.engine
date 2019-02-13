using kedi.engine.Services.Analyzers;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class FinalizerQueueAnalyzerController : ApiController
    {
        FinalizerQueueAnalyzer analyzer = new FinalizerQueueAnalyzer();

        [Route("api/analyzers/finalizer-queue-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }
    }

}
