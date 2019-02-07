using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kedi.engine.MemoryRepresentation
{
    public class MemoryObject
    {
        public ulong Address { get; set; }
        public object Value { get; set; }
        public string TypeName { get; set; }
        public List<ulong> ReferencedObjectsPointers = new List<ulong>();
        public List<ulong> ReferencedByObjectsPointers = new List<ulong>();
        public ulong Size { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if(((MemoryObject)obj).Address == this.Address)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
