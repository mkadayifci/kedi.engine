using kedi.engine.MemoryRepresentation;
using kedi.engine.Services.Analyze;
using System.Linq;

namespace kedi.engine.Services.Analyzers
{

    public class DuplicateStringsAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic Analyze(string sessionId)
        {
            MemoryMap memoryMap = analyzeOrchestrator.GetMemoryMapBySessionId(sessionId);


            return memoryMap.Dictionary
                                    .Where(item => item.Value.TypeName == "System.String")
                                    .GroupBy(item => item.Value.Value.ToString())
                                    .ToList()
                                    .OrderByDescending(item => item.Count())
                                    .Where(item => item.Count() > 1)
                                    .Take(100);
        }


    }
}
