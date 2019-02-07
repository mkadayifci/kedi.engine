using kedi.engine.Services.Analyze;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;

namespace kedi.engine.ConsoleSimulation
{
    [HubName("SignalRHub")]
    public class SignalRHub : Hub
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();
        public void Send(string sessionId, string command)
        {
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
            var debuggerControl = (IDebugControl5)runtime.DataTarget.DebuggerInterface;

            if (command == "l")
            {
                command = @".cordll -lp C:\DumpAnalyze\x64\";
            }
            debuggerControl.ExecuteWide(DEBUG_OUTCTL.THIS_CLIENT, command, DEBUG_EXECUTE.DEFAULT);
            
        }
    }
}