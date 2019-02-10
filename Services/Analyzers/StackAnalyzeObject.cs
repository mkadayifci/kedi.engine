using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.Services.Analyzers
{
    public class StackAnalyzeObject
    {
        public string StackMethodDisplayString { get; set; }
        public string ModuleName { get; set; }
        public int Count { get; set; }
        public List<uint> SeenInThreads = new List<uint>();
        public void AddToSeenThreads(uint threadOSId)
        {
            if (!this.SeenInThreads.Contains(threadOSId))
            {
                this.SeenInThreads.Add(threadOSId);
            }
        }
    }
}
