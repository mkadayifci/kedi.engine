using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Sessions;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace kedi.engine.Services.Analyze
{
    public class AnalyzeOrchestrator : IAnalyzeOrchestrator
    {
        private static object analyzeOrchestratorSync = new object();
        ISessionManager sessionManager = ContainerManager.Container.Resolve<ISessionManager>();

        public Dictionary<string, ClrRuntime> activeRuntimes = new Dictionary<string, ClrRuntime>();
        public Dictionary<string, MemoryMap> activeMemoryMaps = new Dictionary<string, MemoryMap>();

        public MemoryMap GetMemoryMapBySessionId(string sessionId)
        {
            lock (AnalyzeOrchestrator.analyzeOrchestratorSync)
            {
                if (!activeMemoryMaps.ContainsKey(sessionId))
                {
                    var createdMemoryMap = (new MemoryMapGenerator()).GenerateMemoryMap(this.GetRuntimeBySessionId(sessionId));
                    activeMemoryMaps.Add(sessionId, createdMemoryMap);
                }
            }
            return activeMemoryMaps[sessionId];
        }
        public void InitRuntimeBySessionId(string sessionId)
        {
            this.GetRuntimeBySessionId(sessionId);
        }

        public void RemoveAllRuntimes()
        {
            List<string> activeKeys = activeRuntimes.Keys.ToList();
            foreach (var key in activeKeys) 
            {
                this.RemoveRuntimeBySessionId(key);
            }
        }
        public void RemoveRuntimeBySessionId(string sessionId)
        {
            try
            {
                activeRuntimes[sessionId].DataTarget.Dispose();

            }
            catch { }

            try
            {
                activeRuntimes.Remove(sessionId);
                activeMemoryMaps.Remove(sessionId);
            }
            catch { }
        }
        public ClrRuntime GetRuntimeBySessionId(string sessionId)
        {

            lock (AnalyzeOrchestrator.analyzeOrchestratorSync)
            {
                if (!activeRuntimes.ContainsKey(sessionId))
                {
                    Session session = sessionManager.GetById(sessionId);
                    ClrRuntime createdRuntime = CreateRuntime(session.Identifier);

                    //var debuggerInterface = (IDebugClient5)createdRuntime.DataTarget.DebuggerInterface;
                    //var debuggerControl = (IDebugControl5)createdRuntime.DataTarget.DebuggerInterface;
                    //debuggerControl.AddExtension(@"C:\DumpAnalyze\x64\SOS.dll", 0, out ulong handle);

                    ////debuggerControl.AddExtension(@"C:\DumpAnalyze\x64\mscordacwks_amd64_amd64_4.0.30319.34209.dll", 0, out ulong handle2);

                    //var outputCallbacks = new OutputCallbacks();
                    //debuggerInterface.SetOutputCallbacksWide(outputCallbacks);

                    activeRuntimes.Add(sessionId, createdRuntime);
                }
                return activeRuntimes[sessionId];
            }
        }


        public ClrRuntime CreateRuntime(string dumpLocation)
        {
            DataTarget dataTarget = DataTarget.LoadCrashDump(dumpLocation);

            bool isTarget64Bit = dataTarget.PointerSize == 8;
            if (Environment.Is64BitProcess != isTarget64Bit)
                throw new Source32BitException(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

            ClrInfo version = dataTarget.ClrVersions[0];

            string dac = dataTarget.SymbolLocator.FindBinary(version.DacInfo, false);

            if (dac == null || !File.Exists(dac))
            {
                throw new DacNotFoundException("Could not find the specified dac.");
            }

            return version.CreateRuntime(dac);
        }
    }
}
