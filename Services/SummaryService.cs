using kedi.engine.Services.Analyze;
using kedi.engine.Services.Sessions;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;

namespace kedi.engine.Services
{
    public class SummaryService
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();
        ISessionManager sessionManager = ContainerManager.Container.Resolve<ISessionManager>();

        public dynamic GetSummary(string sessionId)
        {
            ClrRuntime clrRuntime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            dynamic returnValue = new
            {
                sessionManager.GetById(sessionId).SourceName,
                Version = clrRuntime.ClrInfo.Version.ToString(),
                Flavor = clrRuntime.ClrInfo.Flavor.ToString(),
                DacInfo = clrRuntime.ClrInfo.DacInfo.PlatformAgnosticFileName,
                TargetArchitecture = clrRuntime.ClrInfo.DacInfo.TargetArchitecture.ToString(),
                AppDomains = new List<dynamic>(),
                Is64Bit = clrRuntime?.DataTarget?.PointerSize == 8,
                GCMode = clrRuntime.ServerGC ? "ServerGC" : "WorkstationGC",
                ThreadCount = clrRuntime.Threads.Count,
                clrRuntime.HeapCount,
                clrRuntime.DataTarget?.DataReader?.IsMinidump
            };

            foreach (var appDomain in clrRuntime.AppDomains)
            {
                returnValue.AppDomains.Add(new
                {
                    appDomain.Name,
                    appDomain.ConfigurationFile
                });
            }

            return returnValue;
        }
    }
}

//clrRuntime.AppDomains
//clrRuntime.Modules
