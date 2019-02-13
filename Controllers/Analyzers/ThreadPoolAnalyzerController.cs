using kedi.engine.Services.Analyzers;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class ThreadPoolAnalyzerController : ApiController
    {
        ThreadPoolAnalyzer analyzer = new ThreadPoolAnalyzer();

        [Route("api/analyzers/threadpool-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }

    }
}



