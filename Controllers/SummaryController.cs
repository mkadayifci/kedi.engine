using kedi.engine.Services;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class SummaryController : ApiController
    {
        SummaryService summaryService = new SummaryService();

        [Route("api/summary/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(summaryService.GetSummary(sessionId));
        }
    }
}
