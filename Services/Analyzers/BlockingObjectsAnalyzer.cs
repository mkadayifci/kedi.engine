using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Analyzers
{
    public class BlockingObjectsAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic Analyze(string sessionId)
        {
            List<dynamic> returnValue = new List<dynamic>();
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            foreach(var blockingObject in runtime.Heap.EnumerateBlockingObjects())
            {
                returnValue.Add(new {
                    ObjectAddress = blockingObject.Object ,
                    blockingObject.Reason,
                    blockingObject.RecursionCount,
                    Locked=blockingObject.Taken,
                    OwnerThreadCount = blockingObject.Owners.Count,
                    WaiterThreadCount = blockingObject.Waiters.Count
                });

            }
            //foreach (var handle in runtime.EnumerateHandles())
            //{
            //    if (handle.IsPinned)
            //    {
            //        returnValue.Add(new
            //        {
            //            ObjectAddress = handle.Object,
            //            TypeName = handle.Type.Name,
            //            handle.HandleType
            //        });
            //    }
            //}
            return returnValue;
        }
    }
}
