using Market.DataObject;
using Microsoft.EntityFrameworkCore;

namespace Market.ORM
{
    public class DBRepositoryPartialKeyDouble<K, V> : DBRepositoryPartial<K, V> where K : notnull, System.IComparable<K> where V : class
    {
        private Object key2;
        private int pos;
        public DBRepositoryPartialKeyDouble(DbSet<V> dbSet, Predicate<V> filter, Object key2, int pos) : base(dbSet, filter)
        {
            this.key2 = key2;
            this.pos = pos;
        }

        //public override V? this[K id]
        //{
        //    get
        //    {
        //        var v = Find(id);
        //        if (v == null || !_filter(v))
        //            return null;
        //        return v;
        //    }
        //    set
        //    {
        //        if (!_filter(value)) { throw new Exception("vallue not relate to this set"); }
        //        if (value == null) throw new ArgumentNullException("value null");
        //        var v = Find(id);
        //        if (v == null)
        //            addItem(value, id);
        //        else
        //            updateItem(value, id);
        //    }
        //}


        protected override V Find(K key1)
        {
            if (pos == 1)
            {
                return dbSet.Find(key2 , key1);
            }
            else
                return dbSet.Find(key1, key2);

        }
    }
}
