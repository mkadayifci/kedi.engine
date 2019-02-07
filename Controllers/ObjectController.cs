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


        //private string GetOutput(ulong obj, GCHeapInstanceField field)
        //{
        //    // If we don't have a simple value, return the address of the field in hex.
        //    if (!field.HasSimpleValue)
        //        return field.GetFieldAddress(obj).ToString("X");

        //    object value = field.GetFieldValue(obj);
        //    if (value == null)
        //        return "{error}";  // Memory corruption in the target process.

        //    // Decide how to format the string based on the underlying type of the field.
        //    switch (field.ElementType)
        //    {
        //        case ClrElementType.ELEMENT_TYPE_STRING:
        //            // In this case, value is the actual string itself.
        //            return (string)value;

        //        case ClrElementType.ELEMENT_TYPE_ARRAY:
        //        case ClrElementType.ELEMENT_TYPE_SZARRAY:
        //        case ClrElementType.ELEMENT_TYPE_OBJECT:
        //        case ClrElementType.ELEMENT_TYPE_CLASS:
        //        case ClrElementType.ELEMENT_TYPE_FNPTR:
        //        case ClrElementType.ELEMENT_TYPE_I:
        //        case ClrElementType.ELEMENT_TYPE_U:
        //            // These types are pointers.  Print as hex.
        //            return string.Format("{0:X}", value);

        //        default:
        //            // Everything else will look fine by simply calling ToString.
        //            return value.ToString();
        //    }
        //}


        private static bool GetPathToObject(ClrHeap heap, ulong objectPointer, Stack<ulong> stack, HashSet<ulong> touchedObjects)
        {
            // Start of the journey - get address of the first objetc on our reference chain
            var currentObject = stack.Peek();

            // Have we checked this object before?
            if (!touchedObjects.Add(currentObject))
            {
                return false;
            }

            // Did we find our object? Then we have the path!
            if (currentObject == objectPointer)
            {
                return true;
            }


            // Enumerate internal references of the object
            var found = false;
            var type = heap.GetObjectType(currentObject);
            if (type != null)
            {
                type.EnumerateRefsOfObject(currentObject, (innerObject, fieldOffset) =>
                {
                    if (innerObject == 0 || touchedObjects.Contains(innerObject))
                    {
                        return;
                    }

                    // Push the object onto our stack
                    stack.Push(innerObject);
                    if (GetPathToObject(heap, objectPointer, stack, touchedObjects))
                    {
                        found = true;
                        return;
                    }

                    // If not found, pop the object from our stack as this is not the tree we're looking for
                    stack.Pop();
                });
            }

            return found;
        }

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
                var mt = runtime.Heap.GetMethodTable(1083183186312);
                var t = runtime.Heap.GetTypeByName("System.DateTime");
                string fieldType = field.Type == null ? "<TYPE>" : field.Type.Name;
                dynamic fieldDetail = new
                {
                    field.Name,
                    field.ElementType,
                    field.IsPrimitive,
                    field.IsObjectReference ,
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
