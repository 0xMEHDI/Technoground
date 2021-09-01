using System.Linq;

namespace SGoap
{
    using System.Collections.Generic;

    public class Inventory : IInventory
    {
        public bool Empty => Items.Count == 0;

        public List<IItem> Items { get; private set; } = new List<IItem>();

        public void Add(IItem item)
        {
            Items.Add(item);
        }

        public void Remove(IItem item)
        {
            Items.Remove(item);
        }

        public void Transfer(IItem item, IInventory inventory)
        {
            Remove(item);
            inventory.Add(item);
        }

        public List<T> GetListOfType<T>()
        {
            return Items.OfType<T>().ToList();
        }
    }
}