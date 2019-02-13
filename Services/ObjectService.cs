using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services
{
    public class ObjectService
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic GetObjectDetail(string sessionId, ulong objPtr)
        {
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
            MemoryMap memoryMap = analyzeOrchestrator.GetMemoryMapBySessionId(sessionId);
            ClrType type = runtime.Heap.GetObjectType(objPtr);
            ClrObject clrObject = new ClrObject(objPtr, type);

            MemoryObject memoryObjectForCurrent = null;
            if (memoryMap.Dictionary.ContainsKey(objPtr))
            {
                memoryObjectForCurrent = memoryMap.Dictionary[objPtr];
            }

            var returnValue = new
            {
                ObjectPointer = objPtr,
                clrObject.HexAddress,
                clrObject.Size,
                clrObject.IsArray,
                clrObject.IsBoxed,
                clrObject.IsNull,
                memoryObjectForCurrent?.ReferencedByObjectsPointers,
                memoryObjectForCurrent?.ReferencedObjectsPointers,
                BaseTypeName = type.BaseType?.Name,
                type.MethodTable,
                ElementType = type.ElementType.ToString(),
                TypeName = type.Name,
                //ObjectValue = type.GetValue(objPtr),
                Members = new List<dynamic>(),
                Values = new List<DateTime>(),
                Module = type.Module.Name
            };

            try
            {
                returnValue.Values.Add(clrObject.GetField<DateTime>("m_start"));
            }
            catch (Exception ex) { }

            foreach (ClrInstanceField field in type.Fields)
            {
                var mt = runtime.Heap.GetMethodTable(1083183186312);
                var t = runtime.Heap.GetTypeByName("System.DateTime");
                string fieldType = field.Type == null ? "<TYPE>" : field.Type.Name;
                dynamic fieldDetail = new
                {
                    field.Name,
                    field.ElementType,
                    field.IsPrimitive,
                    field.IsObjectReference,
                    field.HasSimpleValue,
                    field.Offset,
                    field.IsPublic,
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
