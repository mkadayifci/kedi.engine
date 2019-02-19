using kedi.engine.MemoryRepresentation;
using Microsoft.Diagnostics.Runtime;

namespace kedi.engine.Services.Analyze
{
    public interface IAnalyzeOrchestrator
    {
        ClrRuntime CreateRuntime(string dumpLocation);
        ClrRuntime GetRuntimeBySessionId(string sessionId);
        void InitRuntimeBySessionId(string sessionId);
        void RemoveRuntimeBySessionId(string sessionId);
        void RemoveAllRuntimes();
        MemoryMap GetMemoryMapBySessionId(string sessionId);
    }
}
