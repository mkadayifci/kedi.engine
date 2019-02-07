using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class ObjectMemoryImpactAnalyzerController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [Route("api/analyzers/exceptiogn-analyzer/{sessionId}")]
        [HttpGet]
        public List<dynamic> Get([FromUri]string sessionId)
        {
            List<dynamic> returnValue = new List<dynamic>();
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            foreach (var objPointer in runtime.Heap.EnumerateObjectAddresses())
            {
                var type = runtime.Heap.GetObjectType(objPointer);

                if (type.IsException)
                {
                    var exceptionObject = runtime.Heap.GetExceptionObject(objPointer);
                    var exceptionDetail = (new
                    {

                        ObjectPointer = exceptionObject.Address,
                        TypeName = exceptionObject.Type.Name,
                        exceptionObject.Message,
                        exceptionObject.HResult,
                        Method = exceptionObject.StackTrace?.LastOrDefault<ClrStackFrame>()?.DisplayString,
                        StackTrace = new List<dynamic>()
                    });



                    foreach (ClrStackFrame frame in exceptionObject.StackTrace)
                    {
                        exceptionDetail.StackTrace.Add(new
                        {
                            frame.DisplayString,
                            frame.ModuleName,
                        });
                    }
                    returnValue.Add(exceptionDetail);
                }
            }
            return returnValue;
        }

    }
}



