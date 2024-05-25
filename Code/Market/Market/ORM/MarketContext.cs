using Market.DataObject;
using Market.DomainLayer.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Market.Pages.Modles;

namespace Market.ORM
{
    /*For update DB:
        Add-Migration InitialCreate
        Update-Database
    */

    public class MarketContext : DbContext
    {
        //tables
        public DbSet<DomainLayer.Users.Alert> Alerts { get; set; }
        public DbSet<DomainLayer.Users.Member> Members { get; set; }
        public DbSet<DomainLayer.Market.AppointeesTrees.AppointeeNode> AppointeeNodes { get; set; }
        public DbSet<DomainLayer.Market.Shop> Shops { get; set; }
        //public DbSet<DomainLayer.Market.StockItem> StockItems { get; set; }
        public DbSet<DomainLayer.Market.ShopBasket> ShopBaskets { get; set; }       
        public DbSet<DomainLayer.Market.ShopBasketItem> ShopBasketsItems { get; set; }

        public DbSet<DomainLayer.Market.Item> Items { get; set; }
        public DbSet<DomainLayer.Market.UserPermissions.Permissions> Permissions { get; set; }

        public DbSet<DataObject.OrderRecords.PastOrder> ShopPastOrders { get; set; }
        //public DbSet<DataObject.OrderRecords.PastUserOrder> UserPastOrders { get; set; }



        public string DbPath { get; }
        private static MarketContext? instance;
        public static MarketContext? Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MarketContext();
                }
                return instance;
            }
        }

        public MarketContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Combine(path, "database.db");
            //DbPath = System.IO.Path.Join(path, "database.db");
            Database.EnsureCreated();
            //FormattableString cmd = $"SET IDENTITY_INSERT Shops ON";
            //Database.ExecuteSql(cmd);
            SaveChanges();
            //Database.Migrate();
        }
        public MarketContext(DbContextOptions options) : base(options)
        {
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        //=> options.UseSqlite($"Data Source={DbPath}");
        => options.UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;Database=market;MultipleActiveResultSets=true");
        //=> options.UseSqlServer(builder.Configuration.GetConnectionString("MarketContext"));


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //}

        public bool ResetDB()
        {
            try
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteAllRowsOfTable<T>(DbSet<T> dbSet) where T : class
        {
            try
            {
                string tableName = dbSet.GetType().Name;
                FormattableString cmd = $"DELETE FROM {tableName}";
                this.Database.ExecuteSql(cmd);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveAllRowsOfTable<T>(DbSet<T> dbSet) where T : class
        {
            try
            {
                
                if (dbSet.Any())
                {
                    dbSet.RemoveRange(dbSet.ToList());
                }
                this.SaveChanges();

                foreach (var item in dbSet)
                {
                    dbSet.Remove(item);
                }
                this.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DbSet<Market.Pages.Modles.UserModel>? UserModel { get; set; }
    }
}
