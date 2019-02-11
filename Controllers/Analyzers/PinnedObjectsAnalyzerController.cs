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
  public  class PinnedObjectsAnalyzerController : ApiController
    {

        [Route("api/analyzers/pinned-objects-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            var analyzer = new PinnedObjectAnalyzer();
            var result = analyzer.Analyze(sessionId);

            return Ok(result);
        }
    }

}



