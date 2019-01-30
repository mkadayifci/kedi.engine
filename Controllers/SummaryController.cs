using kedi.engine.Services.Analyze;
using kedi.engine.Services.Sessions;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class SummaryController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();
        ISessionManager sessionManager = ContainerManager.Container.Resolve<ISessionManager>();

        [HttpGet]
        [Route("api/summary/{sessionId}")]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            ClrRuntime clrRuntime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            dynamic returnValue = new
            {
                SourceName= sessionManager.GetById(sessionId).SourceName,
                Version = clrRuntime.ClrInfo.Version.ToString(),
                Flavor = clrRuntime.ClrInfo.Flavor.ToString(),

                DacInfo = clrRuntime.ClrInfo.DacInfo.PlatformAgnosticFileName,
                TargetArchitecture = clrRuntime.ClrInfo.DacInfo.TargetArchitecture.ToString(),

                AppDomains = new List<dynamic>()

            };

            foreach (var appDomain in clrRuntime.AppDomains)
            {
                returnValue.AppDomains.Add(new
                {
                    appDomain.Name,
                    appDomain.ConfigurationFile
                });
            }

            //clrRuntime.AppDomains
            //clrRuntime.Modules
            return Ok<dynamic>(returnValue);
        }
    }
}

