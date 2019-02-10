using kedi.engine.Services.Analyze;
using kedi.engine.Services.Analyzers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class FinalizerQueueAnalyzerController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [Route("api/analyzers/finalizer-queue-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            var analyzer = new FinalizerQueueAnalyzer();
            var result = analyzer.Analyze(sessionId);

            return Ok(result);
        }
    }

}
