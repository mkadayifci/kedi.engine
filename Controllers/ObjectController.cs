using kedi.engine.Services;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class ObjectController : ApiController
    {
        ObjectService objectService = new ObjectService();

        [HttpGet]
        [Route("api/object/{sessionId}/{objPtr}")]
        public IHttpActionResult Get([FromUri]string sessionId, [FromUri]ulong objPtr)
        {
            return Ok(objectService.GetObjectDetail(sessionId,objPtr));
        }
    }
}
