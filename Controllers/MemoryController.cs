using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
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
                    StatsByType = new Dictionary<string, HeapTypeStat>()

                };

                for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                {
                    // This gets the type of the object.
                    ClrType type = runtime.Heap.GetObjectType(obj);
                    ulong size = type.GetSize(obj);
                    if (segmentInfo.StatsByType.ContainsKey(type.Name))
                    {
                        segmentInfo.StatsByType[type.Name].Count = segmentInfo.StatsByType[type.Name].Count + 1;
                    }
                    else
                    {
                        segmentInfo.StatsByType.Add(type.Name, new HeapTypeStat() { Count = 1 });

                    }
                }


                returnValue.Heaps.Add(segmentInfo);


            }

            return returnValue;
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
    }
}
