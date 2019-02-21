using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
namespace kedi.engine.Services.Analyzers
{
    public class StackTraceAnalyzer
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic Analyze(string sessionId)
        {
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);

            return new
            {
                MethodHitData = this.GetMethodHitData(runtime),
                ExactMatchData = this.GetExactStackTraceMatches(runtime)
            };
        }

        private string GetDisplayString(ClrStackFrame frame)
        {
            return frame.Method != null && frame.DisplayString != frame.Method.ToString() ?
                                  $"[{frame.DisplayString}] {frame.Method.ToString()}" :
                                  frame.DisplayString;
        }

        public dynamic GetExactStackTraceMatches(ClrRuntime runtime)
        {
            var returnValue = new Dictionary<string, List<uint>>();

            foreach (var thread in runtime.Threads)
            {
                var threadStackKey = string.Empty;

                foreach (ClrStackFrame frame in thread.StackTrace)
                {
                    threadStackKey += this.GetDisplayString(frame) + "\n";
                }

                if (string.IsNullOrWhiteSpace(threadStackKey))
                    continue;

                if (!returnValue.ContainsKey(threadStackKey))
                {
                    returnValue.Add(threadStackKey, new List<uint>());
                }

                returnValue[threadStackKey].Add(thread.OSThreadId);
            }

            return returnValue
                    .Where(item => item.Value.Count > 1)
                    .Select(item => new
                    {
                        NumberOfThreads = item.Value.Count,
                        CallStack = item.Key.Split('\n').Where(displayString => !string.IsNullOrEmpty(displayString)),
                        OsThreadIds = item.Value
                    })
                    .ToList();

        }

        public List<StackAnalyzeObject> GetMethodHitData(ClrRuntime runtime)
        {
            var returnValue = new Dictionary<string, StackAnalyzeObject>();

            foreach (var thread in runtime.Threads)
            {
                foreach (ClrStackFrame frame in thread.StackTrace)
                {
                    string key = $"{frame.DisplayString}{frame.ModuleName}";
                    if (returnValue.ContainsKey(key))
                    {
                        var currentItem = returnValue[key];
                        currentItem.Count++;
                        currentItem.AddToSeenThreads(thread.OSThreadId);
                    }
                    else
                    {
                        StackAnalyzeObject newItem = new StackAnalyzeObject()
                        {
                            Count = 1,
                            StackMethodDisplayString = frame.DisplayString,
                            ModuleName = frame.ModuleName
                        };
                        newItem.AddToSeenThreads(thread.OSThreadId);
                        returnValue.Add(key, newItem);
                    }
                }
            }

            return returnValue.Values.
                                    OrderByDescending(item => item.Count).
                                    ToList();
        }
    }
}
