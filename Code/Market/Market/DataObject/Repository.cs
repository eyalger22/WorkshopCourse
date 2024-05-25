using System.Collections;

namespace Market.DataObject
{
    public interface Repository<K,V> //: IEnumerable<<K V>> where K : notnull
    {
        // indexing operator
        
        public V? GetItem(K id);
        public Response<V> AddItem(V item, K id);
        public Response<V> UpdateItem(V item, K id);
        public Response<V> DeleteItem(K id);
        
        public V? this[K id] { get; set; }
        
        //public void addRange(Repository<K, V> other);
        
        public bool ContainsKey(K id);
        
        //public IEnumerable<K> Keys();
        
        public IEnumerable<V> Values();

        public int Count();

        public void RemoveAll();
    }
}


