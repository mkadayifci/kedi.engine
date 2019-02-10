using kedi.engine.Services.Analyze;
using kedi.engine.Services.Analyzers;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class StackTraceAnalyzerController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [Route("api/analyzers/stack-trace-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            var analyzer = new StackTraceAnalyzer();
            var result = analyzer.Analyze(sessionId);

            return Ok(result);
        }
    }

}



