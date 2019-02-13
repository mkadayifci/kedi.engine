using kedi.engine.Services.Analyzers;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class PinnedObjectsAnalyzerController : ApiController
    {
        PinnedObjectAnalyzer analyzer = new PinnedObjectAnalyzer();

        [Route("api/analyzers/pinned-objects-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }
    }

}



