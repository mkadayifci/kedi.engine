using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Analyze;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class DuplicateStringsAnalyzerController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [Route("api/analyzers/duplicate-strings-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            MemoryMap memoryMap = analyzeOrchestrator.GetMemoryMapBySessionId(sessionId);


            var result = memoryMap.Dictionary
                                    .Where(item => item.Value.TypeName == "System.String")
                                    .GroupBy(item => item.Value.Value.ToString())
                                    .ToList()
                                    .OrderByDescending(item => item.Count())
                                    .Where(item => item.Count() > 1)
                                    .Take(100);


            return Ok(result);
        }

    }
}




