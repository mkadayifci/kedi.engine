using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Collections.Generic;
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
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
            ClrType type = runtime.Heap.GetObjectType(objPtr);
            ClrObject clrObject = new ClrObject(objPtr, type);

            var returnValue = new
            {
                ObjectPointer = objPtr,
                clrObject.HexAddress,
                clrObject.Size,
                clrObject.IsArray,
                clrObject.IsBoxed,
                clrObject.IsNull,
                BaseTypeName=type.BaseType?.Name,
                type.MethodTable,
                ElementType= type.ElementType.ToString(),
                TypeName = type.Name,
                ObjectValue = type.GetValue(objPtr),
                Members = new List<dynamic>(),
                Values = new List<DateTime>(),
                Module=type.Module.Name

            };

            try
            {
                returnValue.Values.Add(clrObject.GetField<DateTime>("m_start"));
            }
            catch (Exception ex) { }

            foreach (ClrInstanceField field in type.Fields)
            {
                string fieldType = field.Type == null ? "<TYPE>" : field.Type.Name;
                dynamic fieldDetail = new
                {
                    field.Name,
                    field.ElementType,
                    field.IsPrimitive,
                    field.HasSimpleValue,
                    field.Offset,
                    FieldType = field.Type.Name,
                    Address = field.GetAddress(objPtr),
                    Value = field.GetValue(objPtr)
                };

                if (fieldDetail.Name == "m_start")
                {

                    var gg = fieldDetail.Value;
                }
                try
                {
                    var val = field.GetValue(fieldDetail.Address);
                }
                catch { }
                returnValue.Members.Add(fieldDetail);

            }
            return returnValue;
        }
    }
}
