using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects
{
    public class NonSerializableCollection : ICollection<string>
    {
        private List<string> storage = new List<string>();

        public void Add(string item)
        {
            storage.Add(item);
        }

        public void Clear()
        {
            storage.Clear();
        }

        public bool Contains(string item)
        {
            return storage.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            storage.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return storage.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<string>)storage).IsReadOnly; }
        }

        public bool Remove(string item)
        {
            return storage.Remove(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return storage.GetEnumerator();
        }
    }
}