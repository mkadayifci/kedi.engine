using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;

namespace kedi.engine.Services.Analyzers
{
    public class FinalizerQueueAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic Analyze(string sessionId)
        {

            var returnValue = new List<dynamic>();
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
            
            
            return new
            {
                FinalizableObjects = this.GetFinalizeObjects(runtime, runtime.Heap.EnumerateFinalizableObjectAddresses()),
                ObjectsInFinalizerQueue = this.GetFinalizeObjects(runtime, runtime.EnumerateFinalizerQueueObjectAddresses())
            };
        }

        private dynamic GetFinalizeObjects(ClrRuntime runtime, IEnumerable<ulong> objectAdresses)
        {
            var returnValue = new List<dynamic>();
            foreach (var objectAddressInQueue in objectAdresses)
            {
                var typeOfObject = runtime.Heap.GetObjectType(objectAddressInQueue);
                if (typeOfObject != null)
                {
                    returnValue.Add(new
                    {
                        ObjectAddress = objectAddressInQueue,
                        TypeName = typeOfObject.Name
                    });
                }
            }
            return returnValue;
        }
    }
}
