using System.Collections.Generic;

namespace Ford.SaveSystem
{
    internal class ArraySerializable<T>
    {
        public ICollection<T> Items { get; set; }

        public ArraySerializable(ICollection<T> items)
        {
            Items = items;
        }
    }
}
