using Market.DataObject;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

namespace Market.ORM
{
    public class DBRepository<K, V> : Repository<K, V> where K : notnull, System.IComparable<K> where V : class
    {
        private MarketContext context;
        public DbSet<V> dbSet;

        public DBRepository(DbSet<V> dbSet)
        {
            context = MarketContext.Instance;
            this.dbSet = dbSet;
        }

        public V? this[K id]
        {
            get => dbSet.Find(id); 
            set
            {
                if (value == null) throw new ArgumentNullException("value null");
                var v = dbSet.Find(id);
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
                try
                {
                    //FormattableString sql = $"SET IDENTITY_INSERT {nameof(dbSet)} ON";
                    //context.Database.ExecuteSqlInterpolated(sql);
                    dbSet.Add(item);
                    context.SaveChanges();
                    return new Response<V>(item);
                }
                catch (Exception e)
                {
                    //context.Database.RollbackTransaction();
                    return new Response<V>($"Exception in addItem: value: {item}, key: {id} in table {nameof(context)}", 9);
                }
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

        public bool ContainsKey(K id)
        {
            return dbSet.Find(id) != null;
        }

        public int Count()
        {
            return dbSet.Count();
        }

        public Response<V> DeleteItem(K id)
        {
            lock (this)
            {
                try
                {
                    V v = dbSet.Find(id);
                    dbSet.Remove(v);
                    context.SaveChanges();
                    return new Response<V>(v);
                }
                catch
                {
                    return new Response<V>($"Exception in deleteItem {id} in table {nameof(dbSet.GetType)}", 9);
                }
            }
        }

        public IEnumerator< V> GetEnumerator()
        {
            return Values().GetEnumerator();
        }

        public V? GetItem(K id)
        {
            try
            {
                return dbSet.Find(id);
            }
            catch
            {
                throw new Exception($"Error in find {id} in table {nameof(dbSet.GetType)}");
            }
        }

        public void RemoveAll()
        {
            foreach (var item in Values())
            {
                dbSet.Remove(item);
            }
        }

        public Response<V> UpdateItem(V item, K id)
        {
            lock (this)
            {
                try
                {
                    //using var transaction = context.Database.BeginTransaction();
                    V v = dbSet.Find(id);
                    //dbSet.Remove(v);
                    //dbSet.Add(item);
                    //transaction.Commit();
                    context.SaveChanges();
                    return new Response<V>(item);
                }
                catch
                {
                    //context.Database.RollbackTransaction();
                    return new Response<V>($"Exception in updateItem: value: {item}, key: {id} in table {nameof(dbSet)}", 9);
                }
            }
        }

        public IEnumerable<V> Values()
        {
            return dbSet.ToList();
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return dbSet.ToList().GetEnumerator();
        //}
    }
}
