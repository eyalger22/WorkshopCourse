using Market.DataObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Market.ORM
{
    public class DBRepositoryPartial<K, V> : Repository<K, V> where K : notnull, System.IComparable<K> where V : class
    {
        private MarketContext context;
        public DbSet<V> dbSet;
        protected Predicate<V> _filter;

        public DBRepositoryPartial(DbSet<V> dbSet, Predicate<V> filter)
        {
            context = MarketContext.Instance;
            this.dbSet = dbSet;
            _filter = filter;
        }

        public virtual V? this[K id]
        {
            get
            {
                var v = Find(id);
                if (v == null || !_filter(v))
                    return null;
                return v;
            }
            set
            {
                if (!_filter(value)) { throw new Exception("vallue not relate to this set"); }
                if (value == null) throw new ArgumentNullException("value null");
                var v = Find(id);
                if (v == null)
                    AddItem(value, id);
                else
                    UpdateItem(value, id);
            }
        }

        public Response<V> AddItem(V item, K id)
        {
            lock (this)
            {
                if (!_filter(item)) 
                { 
                    return new Response<V>("value not relate to this set", 6); 
                }
                try
                {
                    dbSet.Add(item);
                    context.SaveChanges();
                    return new Response<V>(item);
                }
                catch
                {
                    return new Response<V>($"Exception in addItem {item}, in table {nameof(dbSet.GetType)}", 9);
                }
            }
        }

        public void RemoveAll()
        {
            foreach (var item in Values())
            {
                dbSet.Remove(item);
            }
        }

        //public void addRange(Repository<K, V> other)
        //{
        //    lock (this)
        //    {
        //        foreach (var item in other)
        //        {
        //            addItem(item.Value, item.Key);
        //        }
        //    }
        //}

        public virtual bool ContainsKey(K id)
        {
            var item = Find(id);
            if (item == null)
            {
                return false;
            }
            if (!_filter(item))
            {
                return false;
            }
            return true;
        }

        public int Count()
        {
            return dbSet.ToList().FindAll(_filter).Count;
        }

        public Response<V> DeleteItem(K id)
        {
            lock (this)
            {
                try
                {
                    V v = GetItem(id);
                    if (v == null)
                    {
                        return new Response<V>("not exist", 1);
                    }
                    dbSet.Remove(v);
                    context.SaveChanges();
                    return new Response<V>(v);
                }
                catch
                {
                    return new Response<V>($"Exception in deleteItem {id}", 9);
                }
            }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public V? GetItem(K id)
        {
            var item = Find(id);
            if (!_filter(item))
            {
                throw new Exception("value not relate to this set");
            }
            return item;
        }


        public Response<V> UpdateItem(V item, K id)
        {
            lock (this)
            {
                if (!_filter(item))
                {
                    return new Response<V>($"value {item} not relate to this set" , 6);
                }
                try
                {
                    //using var transaction = context.Database.BeginTransaction();
                    V v = Find(id);
                    //dbSet.Remove(v);
                    //dbSet.Add(item);
                    //transaction.Commit();

                    context.SaveChanges();
                    return new Response<V>(item);
                }
                catch
                {
                    return new Response<V>($"Exception in updateItem {item}, in table {nameof(dbSet.GetType)}", 9);
                }
            }
        }

        public IEnumerable<V> Values()
        {
            return dbSet.ToList().FindAll(_filter);
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return Values().GetEnumerator();
        //}

        protected virtual V Find(K key)
        {
            
            return dbSet.Find(key);

        }
    }
}
