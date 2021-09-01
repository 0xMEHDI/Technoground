using System.Collections.Generic;

namespace SGoap
{
    public interface IInventory
    {
        List<IItem> Items { get; }
        void Add(IItem item);
        void Remove(IItem item);
        void Transfer(IItem item, IInventory inventory);
        bool Empty { get; }
        List<T> GetListOfType<T>();
    }
}