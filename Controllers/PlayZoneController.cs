using kedi.engine.Services;
using kedi.engine.Services.Analyzers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class PlayZoneController : ApiController
    {
        PlayZone playZone = new PlayZone();

        [Route("api/play-zone/types/{sessionId}")]
        [HttpGet]
        public IHttpActionResult GetTypes([FromUri]string sessionId)
        {
            return Ok(playZone.GetTypes(sessionId));
        }

        [Route("api/play-zone/results/{sessionId}")]
        [HttpGet]
        public IHttpActionResult GetResults([FromUri]string sessionId, [FromUri]string queryValue="", [FromUri]string[] type=default(string[]))
        {
            var result = playZone.GetResults(sessionId,type, queryValue);
            return Ok(result);
        }

    }

}



