using kedi.engine.Services;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class ModulesController : ApiController
    {
        ModulesService modulesService = new ModulesService();

        [HttpGet]
        [Route("api/modules/{sessionId}")]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(modulesService.GetModules(sessionId));
        }
    }
}
