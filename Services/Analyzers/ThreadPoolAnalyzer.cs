using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Analyzers
{
    public class ThreadPoolAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic Analyze(string sessionId)
        {
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            dynamic returnValue = new
            {
                runtime.ThreadPool.CpuUtilization,
                runtime.ThreadPool.FreeCompletionPortCount,
                runtime.ThreadPool.IdleThreads,
                runtime.ThreadPool.MaxCompletionPorts,
                runtime.ThreadPool.MaxFreeCompletionPorts,
                runtime.ThreadPool.MaxThreads,
                runtime.ThreadPool.MinCompletionPorts,
                runtime.ThreadPool.MinThreads,
                runtime.ThreadPool.RunningThreads,
                runtime.ThreadPool.TotalThreads

            };
            foreach (var item in runtime.ThreadPool.EnumerateManagedWorkItems())
            {
            }

            foreach (var item in runtime.ThreadPool.EnumerateNativeWorkItems())
            {

            }


            return returnValue;
        }
    }
}
