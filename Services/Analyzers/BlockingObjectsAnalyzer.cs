using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace kedi.engine.Services.Analyzers
{
    public class BlockingObjectsAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();
        ThreadService threadService = new ThreadService();

        public dynamic Analyze(string sessionId)
        {
            List<dynamic> returnValue = new List<dynamic>();
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            foreach (var blockingObject in runtime.Heap.EnumerateBlockingObjects())
            {
                returnValue.Add(new
                {
                    ObjectAddress = blockingObject.Object,
                    blockingObject.Reason,
                    blockingObject.RecursionCount,
                    Locked = blockingObject.Taken,
                    OwnerThreadCount = blockingObject.Owners.Count,
                    WaiterThreadCount = blockingObject.Waiters.Count,
                    OwnerThreads = GetThreadStacksInfo(blockingObject.Owners),
                    WaiterThreads = GetThreadStacksInfo(blockingObject.Waiters)

                });

            }

            return returnValue;
        }

        private List<dynamic> GetThreadStacksInfo(IList<ClrThread> threads)
        {
            var returnValue = new List<dynamic>();

            foreach (var thread in threads)
            {
                if (thread == null)
                    continue;

                returnValue.Add(new
                {
                    OsThreadId = thread.OSThreadId,
                    StackTrace = threadService.GetStackTrace(thread)
                });
            }
            return returnValue;
        }
    }
}
