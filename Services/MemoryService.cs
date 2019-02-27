using kedi.engine.Services.Analyze;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace kedi.engine.Services
{
    public class MemoryService
    {
        IAnalyzeOrchestrator analyzeOrchestrator = ContainerManager.Container.Resolve<IAnalyzeOrchestrator>();

        public dynamic GetMemoryStats(string sessionId)
        {
            ClrRuntime runtime = analyzeOrchestrator.GetRuntimeBySessionId(sessionId);
            dynamic returnValue = new ExpandoObject();

            InitReturnValue(returnValue);

            returnValue.TotalHeapSize = runtime.Heap.TotalHeapSize;

            foreach (ClrSegment seg in runtime.Heap.Segments)
            {
                returnValue.Gen0Length += seg.Gen0Length;
                returnValue.Gen1Length += seg.Gen1Length;
                if (seg.IsLarge)
                    returnValue.LOHLength += seg.Gen2Length;
                else
                    returnValue.Gen2Length += seg.Gen2Length;
                for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                {
                    ClrType type = runtime.Heap.GetObjectType(obj);
                    var objectsGeneration = seg.GetGeneration(obj);

                    if (type != null)
                    {
                        ulong size = type.GetSize(obj);
                        this.AddToTypeToDictionary(returnValue.StatsByType, type.Name, size);

                        switch (objectsGeneration)
                        {
                            case 0:
                                this.AddToTypeToDictionary(returnValue.StatsByTypeGen0, type.Name, size);
                                break;
                            case 1:
                                this.AddToTypeToDictionary(returnValue.StatsByTypeGen1, type.Name, size);
                                break;
                            case 2:
                                if (seg.IsLarge)
                                    this.AddToTypeToDictionary(returnValue.StatsByTypeGen3, type.Name, size);
                                else
                                    this.AddToTypeToDictionary(returnValue.StatsByTypeGen2, type.Name, size);
                                break;
                        }
                    }
                }

            }

            this.FillPercentageData(returnValue.StatsByType);
            this.FillPercentageData(returnValue.StatsByTypeGen0);
            this.FillPercentageData(returnValue.StatsByTypeGen1);
            this.FillPercentageData(returnValue.StatsByTypeGen2);
            this.FillPercentageData(returnValue.StatsByTypeGen3);

            return returnValue;
        }

        private static void InitReturnValue(dynamic returnValue)
        {
            returnValue.StatsByType = new Dictionary<string, HeapTypeStat>();
            returnValue.StatsByTypeGen0 = new Dictionary<string, HeapTypeStat>();
            returnValue.StatsByTypeGen1 = new Dictionary<string, HeapTypeStat>();
            returnValue.StatsByTypeGen2 = new Dictionary<string, HeapTypeStat>();
            returnValue.StatsByTypeGen3 = new Dictionary<string, HeapTypeStat>();
            returnValue.Gen0Length = ulong.MinValue;
            returnValue.Gen1Length = ulong.MinValue;
            returnValue.Gen2Length = ulong.MinValue;
            returnValue.LOHLength = ulong.MinValue;
        }

        private void FillPercentageData(Dictionary<string, HeapTypeStat> dictionary)
        {
            var totalCount = dictionary.Values.Sum(item => item.Count);

            foreach (var item in dictionary.Values)
            {
                item.Percentage = Math.Round(((double)item.Count / (double)totalCount) * 100, 3);
            }
        }

        private void AddToTypeToDictionary(Dictionary<string, HeapTypeStat> dictionary, string typeName, ulong size)
        {
            if (dictionary.ContainsKey(typeName))
            {
                var currentItem = dictionary[typeName];
                currentItem.Count += 1;
                currentItem.TotalSize += size;
            }
            else
            {
                dictionary.Add(typeName, new HeapTypeStat() { Count = 1, TotalSize = size });
            }
        }

        public class HeapTypeStat
        {
            public int Count { get; set; }
            public double Percentage { get; set; }
            public ulong TotalSize { get; set; }
        }
    }
}
