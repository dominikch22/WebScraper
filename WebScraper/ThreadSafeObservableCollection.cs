using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        private readonly object _lock = new object();

        protected override void InsertItem(int index, T item)
        {
            lock (_lock)
            {
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (_lock)
            {
                base.RemoveItem(index);
            }
        }

        protected override void SetItem(int index, T item)
        {
            lock (_lock)
            {
                base.SetItem(index, item);
            }
        }

        protected override void ClearItems()
        {
            lock (_lock)
            {
                base.ClearItems();
            }
        }
    }
}
