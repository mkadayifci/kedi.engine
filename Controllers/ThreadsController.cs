using kedi.engine.Services;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class ThreadsController : ApiController
    {
        ThreadService threadService = new ThreadService();

        [HttpGet]
        [Route("api/threads/{sessionId}")]
        public IHttpActionResult GetThreads([FromUri]string sessionId)
        {
            return Ok(threadService.GetThreads(sessionId));
        }
    }
}