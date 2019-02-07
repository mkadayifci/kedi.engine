using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.MemoryRepresentation
{
    public class MemoryMap
    {
        public Dictionary<ulong, MemoryObject> Dictionary = new Dictionary<ulong, MemoryObject>();
    }
}
