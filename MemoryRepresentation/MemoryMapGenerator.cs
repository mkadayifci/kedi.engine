using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.MemoryRepresentation
{
    public class MemoryMapGenerator
    {
        public MemoryMap GenerateMemoryMap(ClrRuntime runtime)
        {
            MemoryMap map = new MemoryMap();

            foreach (var root in runtime.Heap.EnumerateRoots())
            {
                if (!map.Dictionary.ContainsKey(root.Object))
                {
                    this.FindRefs(runtime.Heap, root.Object, map);
                }
            }

            return map;
        }

        private void FindRefs(ClrHeap heap, ulong objectPointer, MemoryMap memoryMap, MemoryObject parentObject = null)
        {
            MemoryObject memoryObject = new MemoryObject() { Address = objectPointer };
            if (parentObject != null)
            {
                if (!memoryObject.ReferencedObjectsPointers.Contains(memoryObject.Address))
                    parentObject.ReferencedObjectsPointers.Add(memoryObject.Address);

                if (!memoryObject.ReferencedByObjectsPointers.Contains(parentObject.Address))
                    memoryObject.ReferencedByObjectsPointers.Add(parentObject.Address);
            }
            
            memoryMap.Dictionary.Add(objectPointer, memoryObject);

            ClrType currentType = heap.GetObjectType(objectPointer);
            if (currentType != null)
            {
                
                if (currentType.HasSimpleValue)
                {
                    memoryObject.Value = currentType.GetValue(objectPointer);
                }

                memoryObject.Size = currentType.GetSize(objectPointer);
                memoryObject.TypeName = currentType.Name;

                currentType.EnumerateRefsOfObject(objectPointer, (ulong childObjectAddress, int offset) =>
                {
                    //Check if object processed before
                    if (!memoryMap.Dictionary.ContainsKey(childObjectAddress))
                    {
                        this.FindRefs(heap, childObjectAddress, memoryMap, memoryObject);
                    }
                    else
                    {
                        memoryObject.ReferencedObjectsPointers.Add(memoryMap.Dictionary[childObjectAddress].Address);
                    }
                });
            }
        }
    }
}
