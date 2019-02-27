using kedi.engine.Services.Analyzers;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class ExceptionAnalyzerController : ApiController
    {
        ExceptionAnalyzer analyzer = new ExceptionAnalyzer();

        [Route("api/analyzers/exception-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }    
    }
}