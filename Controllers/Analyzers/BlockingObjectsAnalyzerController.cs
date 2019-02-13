using kedi.engine.Services.Analyzers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class BlockingObjectsAnalyzerController : ApiController
    {
        BlockingObjectsAnalyzer analyzer = new BlockingObjectsAnalyzer();

        [Route("api/analyzers/blocking-objects-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }
    }

}



