using kedi.engine.ConsoleSimulation;
using kedi.engine.Services.Sessions;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Analyze
{
    public class AnalyzeOrchestrator: IAnalyzeOrchestrator
    {
        ISessionManager sessionManager = ContainerManager.Container.Resolve<ISessionManager>();

        public Dictionary<string, ClrRuntime> activeRuntimes = new Dictionary<string, ClrRuntime>();

        public ClrRuntime GetRuntimeBySessionId(string sessionId)
        {
            if(!activeRuntimes.ContainsKey(sessionId))
            {
                Session session = sessionManager.GetById(sessionId);
                ClrRuntime createdRuntime = CreateRuntime(ConfigurationManager.AppSettings["DataPath"] + "\\" + session.Identifier);
                var debuggerInterface = (IDebugClient5)createdRuntime.DataTarget.DebuggerInterface;
                var outputCallbacks = new OutputCallbacks();
                debuggerInterface.SetOutputCallbacksWide(outputCallbacks);

                activeRuntimes.Add(sessionId,createdRuntime);
            }
            return activeRuntimes[sessionId];
        }


        public ClrRuntime CreateRuntime(string dumpLocation)
        {
            DataTarget dataTarget = DataTarget.LoadCrashDump(dumpLocation);

            bool isTarget64Bit = dataTarget.PointerSize == 8;
            if (Environment.Is64BitProcess != isTarget64Bit)
                throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

            ClrInfo version = dataTarget.ClrVersions[0];
            string dac = dataTarget.SymbolLocator.FindBinary(version.DacInfo);

            if (dac == null || !File.Exists(dac))
                throw new FileNotFoundException("Could not find the specified dac.", dac);

            return version.CreateRuntime(dac);
        }
    }
}
