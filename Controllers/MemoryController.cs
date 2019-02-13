using kedi.engine.Services;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class MemoryController : ApiController
    {
        MemoryService memoryService = new MemoryService();
        [HttpGet]
        [Route("api/memory/{sessionId}")]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(memoryService.GetMemoryStats(sessionId));
        }
    }
}
