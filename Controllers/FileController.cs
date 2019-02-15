using kedi.engine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class FileController : ApiController
    {
        FileService fileService = new FileService();

        [Route("api/file-system")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string path="~")
        {
            try
            {
                return Ok(fileService.GetList(path));
            }
            catch(UnauthorizedAccessException)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            catch(Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
    }

}


