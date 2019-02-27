using kedi.engine.Services.Analyzers;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class DuplicateStringsAnalyzerController : ApiController
    {
        DuplicateStringsAnalyzer analyzer = new DuplicateStringsAnalyzer();

        [Route("api/analyzers/duplicate-strings-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }
    }
}