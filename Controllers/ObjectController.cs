using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class ObjectController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [HttpGet]
        [Route("api/object/{sessionId}/{objPtr}")]
        public dynamic Get([FromUri]string sessionId, [FromUri]ulong objPtr)
        {
            var returnValue = new ExpandoObject() as IDictionary<string, Object>;

            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            ClrType type = runtime.Heap.GetObjectType(objPtr);
            returnValue.Add("ObjectValue", type.GetValue(objPtr));
            foreach (ClrInstanceField field in type.Fields)
            {
                string fieldType = field.Type == null ? "<TYPE>" : field.Type.Name;
                dynamic fieldDetail = new
                {
                    field.IsPrimitive,
                    field.HasSimpleValue,
                    field.Offset,
                    Address  = field.GetAddress(objPtr),
                    Value= field.GetValue(objPtr)
                     
                };
                returnValue.Add(field.Name, fieldDetail);
            }
            return returnValue;
        }
    }
}

/*
 
*/
