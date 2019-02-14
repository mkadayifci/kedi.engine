using System.Collections.Generic;

namespace kedi.engine.MemoryRepresentation
{
    public class RelatedMemoryObject
    {
        public ulong BaseAddress { get; set; }
        public ulong Address { get; set; }
        public string FieldName { get; set; }
        public string RelatedType { get; set; }

        public override bool Equals(object obj)
        {
            RelatedMemoryObject targetObject = (RelatedMemoryObject)obj;
            RelatedMemoryObject currentObject = this;

            return  targetObject.Address == currentObject.Address &&
                    targetObject.FieldName == currentObject.FieldName;
        }

        public override int GetHashCode()
        {
            var hashCode = -1335652081;
            hashCode = hashCode * -1521134295 + Address.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FieldName);
            return hashCode;
        }
    }
}
