using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Sessions;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

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
            if (!activeMemoryMaps.ContainsKey(sessionId))
            {
                var createdMemoryMap = (new MemoryMapGenerator()).GenerateMemoryMap(this.GetRuntimeBySessionId(sessionId));
                activeMemoryMaps.Add(sessionId, createdMemoryMap);
            }
            return activeMemoryMaps[sessionId];
        }

        public ClrRuntime GetRuntimeBySessionId(string sessionId)
        {

            lock (AnalyzeOrchestrator.analyzeOrchestratorSync)
            {
                if (!activeRuntimes.ContainsKey(sessionId))
                {
                    Session session = sessionManager.GetById(sessionId);
                    ClrRuntime createdRuntime = CreateRuntime(ConfigurationManager.AppSettings["DataPath"] + "\\" + session.Identifier);


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
                throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

            ClrInfo version = dataTarget.ClrVersions[0];
            string dac = dataTarget.SymbolLocator.FindBinary(version.DacInfo);

            if (dac == null || !File.Exists(dac))
                throw new FileNotFoundException("Could not find the specified dac.", dac);

            return version.CreateRuntime(dac);
        }
    }
}
