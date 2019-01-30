using Microsoft.Diagnostics.Runtime;

namespace kedi.engine.Services.Analyze
{
    public interface IAnalyzeOrchestrator
    {
        ClrRuntime CreateRuntime(string dumpLocation);
        ClrRuntime GetRuntimeBySessionId(string sessionId);
    }
}
