using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class RootAnalyzerController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [Route("api/analyzers/root-analyzer/{sessionId}")]
        [HttpGet]
        public object Get([FromUri]string sessionId)
        {
            MemoryMap map = new MemoryMap();
            List<dynamic> returnValue = new List<dynamic>();
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
            var c = 0;
            foreach (var root in runtime.Heap.EnumerateRoots())
            {

                if (!map.Dictionary.ContainsKey(root.Object))
                {
                    this.FindRefs(runtime.Heap, root.Object, map);
                }
                c++;
            }

            var re = map.Dictionary
                .Where(item => item.Value.TypeName == "System.String")
                .GroupBy(item => item.Value.Value.ToString())
                .ToList()
                .OrderByDescending(item => item.Count())
                .Where(item => item.Count() > 1)
                .Take(200);


            return re;
        }

        private void FindRefs(ClrHeap heap, ulong objectPointer, MemoryMap memoryMap, MemoryObject parentObject = null)
        {
            MemoryObject memoryObject = new MemoryObject() { Address = objectPointer };
            if (parentObject != null)
            {
                if (!memoryObject.ReferencedObjectsPointers.Contains(memoryObject.Address))
                    parentObject.ReferencedObjectsPointers.Add(memoryObject.Address);

                if (!memoryObject.ReferencedByObjectsPointers.Contains(parentObject.Address))
                    memoryObject.ReferencedByObjectsPointers.Add(parentObject.Address);
            }

            memoryMap.Dictionary.Add(objectPointer, memoryObject);

            ClrType currentType = heap.GetObjectType(objectPointer);
            if (currentType != null)
            {
                if (currentType.HasSimpleValue)
                {
                    memoryObject.Value = currentType.GetValue(objectPointer);
                }

                memoryObject.Size = currentType.GetSize(objectPointer);
                memoryObject.TypeName = currentType.Name;

                currentType.EnumerateRefsOfObject(objectPointer, (ulong childObjectAddress, int offset) =>
                {
                    //Check if object processed before
                    if (!memoryMap.Dictionary.ContainsKey(childObjectAddress))
                    {
                        this.FindRefs(heap, childObjectAddress, memoryMap, memoryObject);
                    }
                    else
                    {
                        memoryObject.ReferencedObjectsPointers.Add(memoryMap.Dictionary[childObjectAddress].Address);
                    }
                });
            }
        }
    }
}


