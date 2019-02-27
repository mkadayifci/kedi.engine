using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;

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

            MemoryObject memoryObjectForCurrent = this.GetMemoryObject(objPtr, memoryMap);

            var returnValue = this.GenerateBasicProps(objPtr, clrObject, type, memoryObjectForCurrent);

            if (type.IsArray)
            {

                int arrayLength = type.GetArrayLength(clrObject);
                bool hasElementSimpleValue = type.ComponentType.HasSimpleValue;
                for (int i = 0; i < arrayLength; i++)
                {
                    var addressOfElement = type.GetArrayElementAddress(objPtr, i);
                    if (hasElementSimpleValue)
                    {
                        returnValue.ArrayElements.Add(new { Index = i, IsContainsAddress = false, Address = addressOfElement, Value = type.GetArrayElementValue(objPtr, i) });
                    }
                    else
                    {
                        string computedValue = addressOfElement.ToString();
                        if (type.ComponentType.Name == "System.DateTime")
                        {
                            ulong dateData = (ulong)type.ComponentType.GetFieldByName("dateData").GetValue(addressOfElement);

                            
                            computedValue = this.GetDateTime(dateData).ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz");
                        }

                        returnValue.ArrayElements.Add(new { Index = i, IsContainsAddress = true, Address = addressOfElement, Value = computedValue });
                    }
                }

            }
            else
            {
                returnValue.Members.AddRange(this.GetMembers(type, objPtr));
            }

            return returnValue;
        }
        private DateTime GetDateTime(ulong dateData)
        {
            const ulong DateTimeTicksMask = 0x3FFFFFFFFFFFFFFF;
            const ulong DateTimeKindMask = 0xC000000000000000;
            const ulong KindUnspecified = 0x0000000000000000;
            const ulong KindUtc = 0x4000000000000000;

            long ticks = (long)(dateData & DateTimeTicksMask);
            ulong internalKind = dateData & DateTimeKindMask;

            switch (internalKind)
            {
                case KindUnspecified:
                    return new DateTime(ticks, DateTimeKind.Unspecified);

                case KindUtc:
                    return new DateTime(ticks, DateTimeKind.Utc);

                default:
                    return new DateTime(ticks, DateTimeKind.Local);
            }
        }

        private MemoryObject GetMemoryObject(ulong objectPointer, MemoryMap memoryMap)
        {
            MemoryObject memoryObjectForCurrent = null;
            if (memoryMap.Dictionary.ContainsKey(objectPointer))
            {
                memoryObjectForCurrent = memoryMap.Dictionary[objectPointer];
            }

            return memoryObjectForCurrent;
        }

        private dynamic GenerateBasicProps(ulong objectPointer, ClrObject currentObject, ClrType currentType, MemoryObject currentMemoryMap)
        {
            return new
            {
                ObjectPointer = objectPointer,
                currentObject.HexAddress,
                currentObject.Size,
                currentObject.IsArray,
                currentObject.IsBoxed,
                currentObject.IsNull,
                currentMemoryMap?.ReferencedByObjects,
                currentMemoryMap?.ReferencedObjects,
                BaseTypeName = currentType.BaseType?.Name,
                currentType.MethodTable,
                ElementType = currentType.ElementType.ToString(),
                TypeName = currentType.Name,
                Members = new List<dynamic>(),
                ArrayElements = new List<dynamic>(),
                Values = new List<DateTime>(),
                Module = currentType.Module.Name
            };


        }

        private List<dynamic> GetMembers(ClrType currentType, ulong objectPointer)
        {
            List<dynamic> returnValue = new List<dynamic>();

            foreach (ClrInstanceField field in currentType.Fields)
            {
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
                    Address = field.GetAddress(objectPointer),
                    Value = field.GetValue(objectPointer)
                };

                returnValue.Add(fieldDetail);
            }
            return returnValue;
        }
    }

}
