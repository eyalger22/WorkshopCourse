
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using Market.DataObject;
using Market.DataObject.OrderRecords;
using Market.DomainLayer.Market.ItemsFilters;
using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;
using Market.ORM;
using Microsoft.EntityFrameworkCore;
using OrdersHistory = Market.DataObject.OrderRecords.OrdersHistory;
using User = Market.DomainLayer.Users.User;
using Market.DomainLayer.ExternalServicesAdapters;

namespace Market.DomainLayer.Market
{
    public class Market
    {
        private ItemFilter _itemFilter;
        private Repository<int, Shop> shops;

        //private Repository<int, string> shopsIds;
        private Repository<string, Order> orders;
        private MarketContext dbContext;
        public ExternalService externalService;


        public Market()
        {
            if (Facade.InTestMode)
            {
                shops = new ListRepository<int, Shop>();
                orders = new ListRepository<string, Order>();
            }
            else
            {
                dbContext = MarketContext.Instance;
                shops = new DBRepository<int, Shop>(dbContext.Shops);
                orders = new ListRepository<string, Order>();
            }
            _itemFilter = new ItemFilter();
            externalService = ExternalService.Instance;

        }

        public Shop GetShop(int shopId)
        {
            ValidationCheck.Validate(!shops.ContainsKey(shopId), "Shop does not exist");
            // ValidationCheck.Validate(shops[shopId].IsClosed, "Shop close");
            return shops[shopId];

        }

        public Shop GetShop(string shopName)
        {
            foreach (Shop s in shops.Values())
            {
                if (s.Name == shopName)
                {
                    return s;
                }
            }
            throw new KeyNotFoundException("Shop does not exist");
        }

        private bool IsShopExist(string shopName)
        {
            try
            {
                return GetShop(shopName) != null;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public void AddShop(Shop shop)
        {
            shops.AddItem(shop, shop.ShopId);
        }
        public void AddShop(string shopName, Member founder, string address, string bank)
        {
            lock (this)
            {
                ValidationCheck.Validate(IsShopExist(shopName), "Shop already exists");
                ValidationCheck.Validate(string.IsNullOrWhiteSpace(shopName), "Shop name can't be empty");
                ValidationCheck.Validate(string.IsNullOrWhiteSpace(address), "address can't be empty");
                using (var transaction = MarketContext.Instance.Database.BeginTransaction())
                {
                    try
                    {
                        FormattableString sql = $"SET IDENTITY_INSERT Shops ON";
                        dbContext.Database.ExecuteSqlInterpolated(sql);
                        dbContext.SaveChanges();
                        Shop shop = new Shop(shopName, founder, address, bank);
                        shops.AddItem(shop, shop.ShopId);
                        MarketContext.Instance.SaveChanges();
                        transaction.Commit();
                        //shopsIds[shop.ShopId] = shop.Name;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error in add shop: {shopName}, {e.Message}");
                    }
                }


            }
        }

        public Response<List<(Item, double)>> AttemptPurchaseCart(string userName)
        {
            lock (this)
            {
                List<(Item, double)> items = new List<(Item, double)>();
                foreach (Shop shop in shops.Values())
                {
                    Response<List<(Item, double)>> res = shop.AttemptPurchaseBasket(userName);
                    if(res.ErrorKind == 0)
                    {
                        items.AddRange(res.Value);
                    } else
                    {
                        // TODO: decide what to do if there was an error in 1 shop
                        return new Response<List<(Item, double)>>($"Error in shop {shop.Name}: {res.ErrorMsg}", 1);
                    }
                }
                return new Response<List<(Item, double)>>(items);
            }
        }
        
        public Response<Order> ParchaseCart(string userName, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            bool hasItems = false;
            lock (this)
            {
                AttemptPurchaseCart(userName);
                List<Shop> shops_to_revert = new List<Shop>();
                //double price = 0;
                var str = "";
                Order order = new Order(paymentDetails);
                foreach (Shop shop in shops.Values())
                {
                    Response<bool> res = shop.PurchaseBasket(userName, order);
                    if (!res.HasError)
                    {
                        shops_to_revert.Add(shop);
                        hasItems = true;
                    }
                    else if (res.ErrorKind == 1)
                    {
                        // TODO: decide what to do if there was an error in 1 shop
                        foreach (var s in shops_to_revert)
                        {
                            s.RevertUserBasket(userName);
                        }
                        return new Response<Order>($"Error in shop {shop.Name}: {res.ErrorMsg}", 1);
                    }
                    else
                    {
                        str += $"{res.ErrorMsg}\n";
                    }
                }
                //pay here, and if it fails, revert all shops
                Response<int> pay = externalService.PaymentService.makePayment(paymentDetails);
                if (pay.HasError)
                {
                    foreach (var s in shops_to_revert)
                    {
                        s.RevertUserBasket(userName);
                    }
                    return new Response<Order>("Pay " + pay.ErrorMsg, 1);
                }
                //deliver here, and if it fails, revert all shops
                Response<int> delivery = externalService.DeliveryService.createDelivery(deliveryDetails);
                if (delivery.HasError)
                {
                    foreach (var s in shops_to_revert)
                    {
                        s.RevertUserBasket(userName);
                    }
                    externalService.PaymentService.cancel_pay(pay.Value.ToString());
                    return new Response<Order>("Del " + delivery.ErrorMsg, 1);
                }
                if (!hasItems)
                {
                    return new Response<Order>($"No items in cart\n{str}", 2);
                }
                orders[userName] = order;
                foreach (var shop in shops.Values())
                {
                    var res = shop.RemoveBasket(userName);
                }

                return new Response<Order>(order);
            }
        }

        public List<Shop> GetOpenShops()
        {

            List<Shop> openShops = new List<Shop>();
            foreach (Shop shop in shops.Values())
            {
                if (!shop.IsClosed)
                {
                    openShops.Add(shop);
                }
            }
            return openShops;
        }
        

        public List<Item> FilterItemsByCategory(string category)
        {
            return FilterItemGeneric(items => _itemFilter.ItemFilterByCategory(items, category), GetOpenShops());
        }
        
        public List<Item> FilterItemsByName(string name)
        {
            return FilterItemGeneric(items => _itemFilter.ItemFilterByName(items, name), GetOpenShops());
        }
        
        public List<Item> FilterItemsByPriceRange(double minPrice, double maxPrice)
        {
            return FilterItemGeneric(items => _itemFilter.ItemFilterByPriceRange(items, minPrice, maxPrice), GetOpenShops());
        }
        
        public List<Item> FilterItemsByItemRank(int rank)
        {
            return FilterItemGeneric(items => _itemFilter.ItemFilterByItemRank(items, rank), GetOpenShops());
        }
        
        public List<Item> FilterItemsByShopRank(int rank)
        {
            List<Shop> shops = _itemFilter.ItemFilterByShopRank(GetOpenShops(), rank);
            return FilterItemGeneric(items => items, shops);
        }

        private List<Item> FilterItemGeneric(Func<List<Item>, List<Item>> func,  IEnumerable<Shop> shops)
        {
            List<Item> items = new List<Item>();
            foreach (var shop in shops)
            {
                items.AddRange(shop.GetItems());
            }
            return func(items);
        }
        
        internal void InitSystem()
        {
            bool inTestMode = Facade.InTestMode;
            if (inTestMode)
            {
                //reset db - market part
                dbContext.RemoveAllRowsOfTable(dbContext.Items);
                dbContext.RemoveAllRowsOfTable(dbContext.Shops);
                shops = new ListRepository<int, Shop>();

            }
            else
            {
                shops = new DBRepository<int, Shop>(dbContext.Shops);

            }

        }

        internal Response<int> CloseShop(int shopId, Member founder)
        {
            Shop s = GetShop(shopId);
            if (!founder.HasPermission(s, PermissionsEnum.Permission.CLOSE_SHOP))
            {
                return new Response<int>("You don't have permission to close the shop", 6);
            }

            if (s.Founder.Name == founder.Name)
            {
                lock (this)
                {
                    s.IsClosed = true;
                    //update permissions
                    founder.AddShopPermissions(s, PermissionsEnum.Permission.OPEN_SHOP);
                    founder.RemoveShopPermissions(s, PermissionsEnum.Permission.CLOSE_SHOP);
                    try
                    {
                        s.SendAlertToShopOwners($"shop: {s.Name} closed by {founder.Name}");
                    }
                    catch (Exception e)
                    {
                        return new Response<int>("Error in send alerts to Shop Owners", 1);
                    }
                    s.IsClosed = true;
                    dbContext.SaveChanges();
                    return new Response<int>(shopId);
                }
            }
            else
            {
                return new Response<int>("Only the founder can close the shop", 1);
            }
        }

        internal Response<int> OpenClosedShop(int shopId, Member founder)
        {
            Shop s = GetShop(shopId);
            if (!founder.HasPermission(s, PermissionsEnum.Permission.OPEN_SHOP))
            {
                return new Response<int>("You don't have permission to open the shop", 6);
            }

            if (s.Founder.Name == founder.Name)
            {
                lock (this)
                {
                    s.IsClosed = false;
                    //update permissions
                    founder.AddShopPermissions(s, PermissionsEnum.Permission.CLOSE_SHOP);
                    founder.RemoveShopPermissions(s, PermissionsEnum.Permission.OPEN_SHOP);
                    try
                    {
                        s.SendAlertToShopOwners($"shop {s.Name} opend again by {founder.Name}");
                    }
                    catch (Exception e)
                    {
                        return new Response<int>("Error in send alerts to Shop Owners", 1);
                    }
                    return new Response<int>(shopId);
                }
            }
            else
            {
                return new Response<int>("Only the founder can open the shop", 1);
            }
        }

        internal Item GetProduct(int productId)
        {
            foreach (Shop shop in shops.Values())
            {
                if (shop.Stock.HasItem(productId))
                {
                    Item item = shop.Stock.GetItem(productId);
                    return item;
                }
            }
            return null;
        }

        private Order? GetOrder(int orderId)
        {
            foreach (Order order in orders.Values())
            {
                if (order.Id == orderId)
                {
                    return order;
                }
            }
            return null;
        }

        internal Response<int> CreateDelivery(string name, string address, string city, string country, string zip)
        {
            lock (this)
            {
                DeliveryDetails details = new DeliveryDetails(name, address, city, country, zip);
                return ExternalServicesAdapters.ExternalService.Instance.DeliveryService.createDelivery(details); 
            }
        }

        internal Response<double> GetUserOrderPrice(string userName)
        {
            var order = orders[userName];
            if(order == null)
            {
                return new Response<double>("User does not have an order", 1);
            }
            return new Response<double>(order.Price);
        }

        internal Response<int> MakePayment(string shop_bank, PaymentDetails customer_payment, double amount, User user)
        {
            lock (this)
            {
                Order order = orders[user.Name];
                if(order.Price != amount)
                {
                    return new Response<int>("The amount is not correct", 1);
                }
                string pay_details = customer_payment.ToString();
                Response<int> res = ExternalServicesAdapters.ExternalService.Instance.PaymentService.makePayment(customer_payment);
                if (res.HasError)
                {
                    orders.DeleteItem(user.Name);
                }
                else
                {
                    foreach (var shop in shops.Values())
                    {
                        shop.AfterPurchase(true, user.Name);
                    }
                    PastUserOrder pastOrder = new PastUserOrder(order);
                    user.AddPastOrder(pastOrder);
                }

                return res;
            }
        }

        internal List<Item> SearchProducts(string? productName, int? productId, int? minPrice, int? maxPrice, string? category)
        {
            List<Item> items = GetAllItems();
            List<Item> new_items;
            if (productId != null)
            {
                Item item = GetProduct((int)productId);
                if (item != null)
                {
                    items.Add(item);
                }
                //return items;
            }
            else
            {
                new_items = GetAllItems();
                items = items.Intersect(new_items).ToList();
            }

            if (productName != null)
            {
                new_items = FilterItemsByName(productName);
                items = items.Intersect(new_items).ToList();
                //items = items.FindAll(item=> new_items.Contains(item));
            }
            else
            {
                items = GetAllItems();
            }
            if (minPrice != null && maxPrice != null)
            {
                new_items = FilterItemsByPriceRange((double)minPrice, (double)maxPrice);
                items = items.Intersect(new_items).ToList();
            }
            else
            {
                new_items = GetAllItems();
                items = items.Intersect(new_items).ToList();

            }

            if (category != null)
            {
                new_items = FilterItemsByCategory(category);
                items = items.Intersect(new_items).ToList();
            }
            else
            {
                new_items = GetAllItems();
                items = items.Intersect(new_items).ToList();
            }
            return items;

        }

        private List<Item> GetAllItems()
        {
            List<Item> items = new List<Item>();
            foreach (Shop shop in GetAllOpenShops())
            {
                items.AddRange(shop.Stock.GetItems());
            }
            return items;
        }

        public Response<OrdersHistory> GetShopOrderHistory(int shopId)
        {
            Shop shop = GetShop(shopId);
            return new Response<OrdersHistory>(shop.OrderHistory);
        }

        internal Response<bool> CloseShopForever(Shop shop)
        {

            lock (this)
            {
                shop.Clear();
                shops.DeleteItem(shop.ShopId);
                try
                {
                    shop.SendAlertToShopOwners($"shop {shop.Name} closed by system manager");
                }
                catch (Exception e)
                {
                    return new Response<bool>("Error in send alerts to Shop Owners", 1);
                }
                return new Response<bool>(true);
            }

        }

        internal List<Shop> GetAllOpenShops()
        {
            List<Shop> openShops = new List<Shop>();
            foreach (var shop in shops.Values())
            {
                if (!shop.IsClosed)
                {
                    openShops.Add(shop);
                }
            }
            return openShops;
        }
        public List<(Product,int)> GetShopProducts(int shopId)
        {
            Shop shop = GetShop(shopId);
            List<(Product, int)> products= new List<(Product, int)> ();
            List<(Item, int)> items = shop.GetProducts();
            foreach((Item,int) i in items)
            {
                Product p = new Product(i.Item1.ItemId,i.Item1.Name,i.Item1.Price,i.Item1.Category,i.Item1.Description,shop.Name);
                products.Add((p, i.Item2));
            }
            return products;
        }

        public List<Product> GetShopProductsWithoutQuantity(int shopId)
        {
            Shop shop = GetShop(shopId);
            List<Product> products = new List<Product>();
            List<(Item, int)> items = shop.GetProducts();
            foreach ((Item, int) i in items)
            {
                Product p = new Product(i.Item1.ItemId, i.Item1.Name, i.Item1.Price, i.Item1.Category, i.Item1.Description);
                products.Add(p);
            }
            return products;
        }
    }
}
