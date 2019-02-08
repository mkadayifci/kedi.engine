using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace kedi.engine.Controllers
{
    public class MemoryController : ApiController
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        [HttpGet]
        [Route("api/memory/{sessionId}")]
        public dynamic Get([FromUri]string sessionId)
        {
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
            var returnValue = new
            {
                Heaps = new List<dynamic>(),
                StatsByType = new Dictionary<string, HeapTypeStat>(),
                runtime.Heap.TotalHeapSize
            };

            foreach (ClrSegment seg in runtime.Heap.Segments)
            {
                var segmentInfo = new
                {
                    HeapSegmentType = this.GetSegmentType(seg),
                    seg.ProcessorAffinity,
                    seg.Gen0Length,
                    seg.Gen1Length,
                    seg.Gen2Length,
                    StatsByType = new Dictionary<string, HeapTypeStat>(),
                };

                for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                {
                    // This gets the type of the object.
                    ClrType type = runtime.Heap.GetObjectType(obj);
                    if (type != null)
                    {
                        ulong size = type.GetSize(obj);
                        //type.EnumerateRefsOfObject(obj, (childAddress, offsett) => { Console.WriteLine(childAddress); });
                        this.AddToTypeToDictionary(segmentInfo.StatsByType, type.Name,size);
                        this.AddToTypeToDictionary(returnValue.StatsByType, type.Name,size);
                    }
                }


                returnValue.Heaps.Add(segmentInfo);


            }

            return returnValue;
        }
        private void AddToTypeToDictionary(Dictionary<string, HeapTypeStat> dictionary,string typeName,ulong size)
        {
            if (dictionary.ContainsKey(typeName))
            {
                var currentItem = dictionary[typeName];
                currentItem.Count += 1;
                currentItem.TotalSize += size;
            }
            else
            {
                dictionary.Add(typeName, new HeapTypeStat() { Count = 1,TotalSize=size });
            }

        }
        private string GetSegmentType(ClrSegment segment)
        {
            if (segment.IsEphemeral)
                return "Ephemeral";
            else if (segment.IsLarge)
                return "Large";
            else
                return "Gen2";
        }
    }
    public class HeapTypeStat
    {
        public int Count { get; set; }
        public ulong TotalSize { get; set; }
    }
}
