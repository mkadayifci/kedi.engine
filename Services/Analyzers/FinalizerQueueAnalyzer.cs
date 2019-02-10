using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Diagnostics;

namespace kedi.engine.Services.Analyzers
{
    public class FinalizerQueueAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic Analyze(string sessionId)
        {
            var returnValue = new List<dynamic>();
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            foreach (var objectAddressInQueue in runtime.Heap.EnumerateFinalizableObjectAddresses())
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
                else
                {
                    Debug.Assert(false);
                }
            }
            return returnValue;
        }
    }
}
