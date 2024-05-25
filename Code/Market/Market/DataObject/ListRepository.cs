using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using Market.DomainLayer.Market.Validation;

namespace Market.DataObject
{
    public class ListRepository<K,V> : Repository<K,V> where K : notnull
    {
        private ConcurrentDictionary<K,V> list;
        
        // indexing operator
        public V? this[K id]
        {
            get => GetItem(id);
            set
            {
                Response<V> res;
                if (list.ContainsKey(id))
                    res = UpdateItem(value, id);
                else
                    res = AddItem(value, id);
                ValidationCheck.Validate(res.ErrorKind != 0, res.ErrorMsg);
            }
        }

        public ListRepository()
        {
            list = new ConcurrentDictionary<K, V>();
        }
        
        public IEnumerable<K> Keys()
        {
            return list.Keys;
        }
        
        public IEnumerable<V> Values()
        {
            return list.Values;
        }
        public Response<V> AddItem(V item, K id)
        {
            if (list.ContainsKey(id))
                return new Response<V>("Item already exists", 1);
            list.AddOrUpdate(id, item, (k, v) => item);
            return new Response<V>(item);
        }
        public Response<V> DeleteItem(K id)
        {
            if (!list.ContainsKey(id))
                return new Response<V>("Item does not exist", 1);
            V val = list[id];

            list.Remove(id, out val);
            return new Response<V>(val);
        }
        public V? GetItem(K id)
        {
            if (!list.ContainsKey(id))
                return default(V);
            return list[id];
        }
        public Response<V> UpdateItem(V item, K id)
        {
            if (!list.ContainsKey(id))
                return new Response<V>("Item does not exist", 1);
            list[id] = item;
            return new Response<V>(item);
        }
        
        public int Count()
        {
            return list.Count;
        }

        public IEnumerator<KeyValuePair<K,V>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        //public void addRange(Repository<K, V> other)
        //{
        //    foreach (var item in other)
        //    {
        //        Response<V> res = addItem(item.Value, item.Key);
        //    }
        //}
        
        public bool ContainsKey(K id)
        {
            return list.ContainsKey(id);
        }

        public void RemoveAll()
        {
            list.Clear();
        }
    }
}
