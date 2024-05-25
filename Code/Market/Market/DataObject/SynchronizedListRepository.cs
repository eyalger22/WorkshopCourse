using System.Collections;
using System.Collections.Generic;

namespace Market.DataObject
{
    public class SynchronizedListRepository<K,V> : Repository<K,V> where K : notnull
    {
        private ListRepository<K,V> _listRepository;
        private object _lock;
        public SynchronizedListRepository()
        {
            _listRepository = new ListRepository<K,V>();
            _lock = new object();
        }

        public Response<V> AddItem(V item, K id)
        {
            lock(_lock ) { return _listRepository.AddItem(item,id); }
        }

        public Response<V> DeleteItem(K id)
        {
            lock (_lock) { return  _listRepository.DeleteItem(id); }
        }

        public V? this[K id]
        {
            get => GetItem(id);
            set
            {

                lock (_lock) { _listRepository[id] = value; }
            }
        }

        //public void addRange(Repository<K, V> other)
        //{
        //    lock (_lock) { _listRepository.addRange(other); }
        //}

        public bool ContainsKey(K id)
        {
            lock (_lock) { return _listRepository.ContainsKey(id); }
        }

        public IEnumerable<K> Keys()
        {
            lock (_lock)
            {
                return _listRepository.Keys();
            }
        }

        public IEnumerable<V> Values()
        {
            lock (_lock)
            {
                return _listRepository.Values();
            }
        }

        public int Count()
        {
            lock (_lock)
            {
                return _listRepository.Count();
            }
        }

        public V? GetItem(K id)
        {
            lock (_lock) { return _listRepository.GetItem(id); }
        }

        public Response<V> UpdateItem(V item, K id)
        {
            lock (_lock) { return _listRepository.UpdateItem(item,id); }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            lock (_lock)
            {
                return _listRepository.GetEnumerator();
            }
        }
        public void RemoveAll()
        {
            _listRepository.RemoveAll();
        }
        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
