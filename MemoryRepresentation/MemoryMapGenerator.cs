﻿using Microsoft.Diagnostics.Runtime;

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

        private void FindRefs(ClrHeap heap, ulong objectPointer, MemoryMap memoryMap, MemoryObject parentObject = null, string parentObjectField = "")
        {
            MemoryObject currentObject = new MemoryObject() { Address = objectPointer };
            //Only null if it is a root object in the heap
            if (parentObject != null)
            {
                //It is the object that referenced by parentObjectField at currentObject.Address
                RelatedMemoryObject relatedObject = new RelatedMemoryObject()
                {
                    Address = currentObject.Address,
                    FieldName = parentObjectField,
                    RelatedType=parentObject.TypeName??string.Empty,
                    BaseAddress= parentObject.Address
                };
                if (!currentObject.ReferencedByObjects.Contains(relatedObject))
                {
                    currentObject.ReferencedByObjects.Add(relatedObject);
                }
            }

            memoryMap.Dictionary.Add(objectPointer, currentObject);

            ClrType currentType = heap.GetObjectType(objectPointer);
            if (currentType != null)
            {

                if (currentType.HasSimpleValue)
                {
                    currentObject.Value = currentType.GetValue(objectPointer);
                }

                currentObject.Size = currentType.GetSize(objectPointer);
                currentObject.TypeName = currentType.Name;

                currentType.EnumerateRefsOfObject(objectPointer, (ulong childObjectAddress, int offset) =>
                {

                    string fieldName = this.GetFieldName(currentType, offset);
                    if (!memoryMap.Dictionary.ContainsKey(childObjectAddress))
                    {
                        this.FindRefs(heap, childObjectAddress, memoryMap, currentObject, fieldName);
                    }
                        RelatedMemoryObject relatedObject = new RelatedMemoryObject()
                        {
                            Address = childObjectAddress,
                            FieldName = fieldName,
                            RelatedType = currentObject.TypeName,
                            BaseAddress= objectPointer

                        };

                        currentObject.ReferencedObjects.Add(relatedObject);
                });
            }
        }


        private string GetFieldName(ClrType type, int offset)
        {
            int fuseLimit = 30;
            int fuse = 0;
            int childFieldOffset = 0;
            ClrInstanceField foundField = null;
            bool result = false;
            do
            {
                result = type.GetFieldForOffset(offset, false, out foundField, out childFieldOffset);
                fuse++;
            }
            while ((childFieldOffset != 0 && fuse < fuseLimit));


            return foundField?.Name ?? string.Empty;
        }
    }
}
