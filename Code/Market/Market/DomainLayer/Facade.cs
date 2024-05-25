using Market.DataObject;
using Market.DomainLayer.Market;
using Market.DomainLayer.Market.AppointeesTrees;
using Market.DomainLayer.Market.UserPermissions;
using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;
using User = Market.DomainLayer.Users.User;
using OrdersHistory = Market.DataObject.OrderRecords.OrdersHistory;
using System.Collections.Generic;
using MessagePack;
using Shop = Market.DomainLayer.Market.Shop;
using Market.ORM;
using Microsoft.CodeAnalysis;
using Market.DomainLayer.Hubs;
using Market.DomainLayer.Market.DiscountPolicy;

namespace Market.DomainLayer
{
    public class Facade
    {
        private static Facade? instance;
        internal Market.Market market { get; }
        private UserManager userManager;

        private Facade()
        {
            market = new Market.Market();
            userManager = new UserManager(new SystemManager("admin", "admin", "admin@gmail.com"));
        }

        public static Facade GetFacade()
        {
            if (instance == null)
                instance = new Facade();
            return instance;
        }

        public Response<DataObject.Shop> GetShopByName(string shopName)
        {
            Market.Shop s = market.GetShop(shopName);
            if (s == null)
                return new Response<DataObject.Shop>("shop not found", 6);
            return new Response<DataObject.Shop>(new DataObject.Shop(s.ShopId, s.Name, s.ShopAddress, s.IsClosed));
        }

        public Response<List<DataObject.Shop>> GetAllOpenShops()
        {
            List<Market.Shop> shops = market.GetAllOpenShops();
            List<DataObject.Shop> res = new List<DataObject.Shop>();
            foreach (Market.Shop s in shops)
                res.Add(new DataObject.Shop(s.ShopId, s.Name, s.ShopAddress, s.IsClosed));
            return new Response<List<DataObject.Shop>>(res);
        }

        public Response<bool> UpdateConnectionId(int userId, string connectionId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError)
                return new Response<bool>(res.ErrorMsg, 3);
            if (!(res.Value is Users.Member))
                return new Response<bool>("user is not a member", 6);
            Member user = (Member)res.Value;
            user.ChangeSessionId(connectionId);
            try
            {
                AlertsManager.Instance.SendAlert(user, $"{user.Name}: You are connected");
            }
            catch (Exception e)
            {
                return new Response<bool>(e.Message, 6);
            }
            return new Response<bool>(true);
        }

        public Response<bool> SendAlert(string name,int userId, string msg)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("member not found", 6);
            Member member = (Member)res.Value;
            try{
                AlertsManager.Instance.SendAlert(member, msg);
            }
            catch (Exception e)
            {
                return new Response<bool>(e.Message, 6);
            }
            return new Response<bool>(true);
        }

        public Response<List<string>> ReadAlerts(int userId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<List<string>>("member not found", 6);
            Member member = (Member)res.Value;
            List<string> alerts = member.ReadAlerts();
            return new Response<List<string>>(alerts);
        }

        internal Response<int> AddAmountToProductInStock(int userId, int shopId, int productId, int amount)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<int>("member not found", 6);
            Member member = (Member)res.Value;
            Market.Shop shop = market.GetShop(shopId);
            if (!member.HasPermission(shop, PermissionsEnum.Permission.MANAGE_ITEMS))
                return new Response<int>("member not allowed to add products", 6);
            Stock s = shop.Stock;
            s.AddItemsToStock(productId, amount, member);
            return new Response<int>(s.GetItemQuantity(productId));
        }

        internal Response<int> EditAmountToProductInStock(int userId, int shopId, int productId, int amount)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<int>("member not found", 6);
            Member member = (Member)res.Value;
            Market.Shop shop = market.GetShop(shopId);
            if (!member.HasPermission(shop, PermissionsEnum.Permission.MANAGE_ITEMS))
                return new Response<int>("member not allowed to add products", 6);
            Item item = market.GetProduct(productId);
            if(item != null)
            {
                Stock s = shop.Stock;
                s.EditStock(item, amount);
                return new Response<int>(s.GetItemQuantity(productId));
            }
            return new Response<int>("item cannot be found", 6);
        }

        internal Response<bool> AddManager(int userId, int shopId, string username)
        {
            Response<Users.User> res = userManager.GetUser(username);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't add manager", 4);
            Member manager = (Member)res.Value;
            Response<Users.User> res2 = userManager.GetUser(userId);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<bool>("can't add owner", 4);
            Member member = (Member)res2.Value;
            market.GetShop(shopId).AppointManager(manager, member);
            return new Response<bool>(true);
        }

        internal Response<bool> AddOwner(int userId, int shopId, string username)
        {
            Response<Users.User> res = userManager.GetUser(username);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't add owner", 4);
            Member owner = (Member)res.Value;
            Response<Users.User> res2 = userManager.GetUser(userId);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<bool>("can't add owner", 4);
            Member member = (Member)res2.Value;
            market.GetShop(shopId).AppointOwner(owner, member);
            return new Response<bool>(true);
        }

        internal Response<string> AddPermission(int userId, int shopId, string username,
            PermissionsEnum.Permission permission)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<string>("can't add Permission", 4);
            Member appointer = (Member)res.Value;
            Response<Users.User> res2 = userManager.GetUser(username);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<string>("can't add Permission", 4);
            Member memberToAdd = (Member)res2.Value;
            Market.Shop shop = market.GetShop(shopId);
            if (!appointer.HasPermission(shop, PermissionsEnum.Permission.MANAGE_PERMISSION))
                return new Response<string>("ONLY owners can add permmision", 4);
            if (!shop.CheckAppointeeManager(appointer, username))
            {
                return new Response<string>("ONLY Appointee owner can add permmision", 4);
            }

            memberToAdd.AddShopPermissions(shop, permission);
            return new Response<string>(permission.ToString());
        }

        internal Response<bool> AddProductToCart(int userId, int shopId, int productId, int amount)
        {
            Response<Users.User> u = userManager.GetUser(userId);
            if (u.HasError)
                return new Response<bool>(u.ErrorMsg, 3);
            Market.ShoppingCart cart = u.Value.ShoppingCart;
            Market.Shop shop = market.GetShop(shopId);
            Market.Item item = shop.Stock.GetItem(productId);
            Response<Item> res = cart.AddNewItemToCart(shop.Name, item, amount);
            if (res.HasError)
                return new Response<bool>(res.ErrorMsg, 3);
            shop.AddBasket(cart.GetBasket(shop.Name));
            return new Response<bool>(true);
        }

        internal Response<int> AddProductToShop(int userId, int shopId, string productName, int price, string category,
            string description)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<int>("can't find user", 4);
            Member member = (Member)res.Value;
            if (!member.HasPermission(market.GetShop(shopId), PermissionsEnum.Permission.MANAGE_ITEMS))
                return new Response<int>("member doesn't hava permission to this", 4);
            Market.Shop s = market.GetShop(shopId);
            s.Stock.AddItem(category, productName, price, description);
            return new Response<int>(s.Stock.GetItem(productName).ItemId);
        }

        internal Response<int> CloseShop(int userId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<int>("can't close shop", 4);
            return market.CloseShop(shopId, (Member)res.Value);

        }

        internal Response<int> OpenClosedShop(int userId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<int>("can't open shop", 4);
            return market.OpenClosedShop(shopId, (Member)res.Value);
        }

        internal Response<int> createDelivery(string name, string address, string city, string country, string zip)
        {
            return market.CreateDelivery(name,address,city,country,zip);
        }

        internal Response<bool> EditProductDetails(int userId, int shopId, int productId, string? productName,
            int? price, string? category, string? description)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't find user", 4);
            Member member = (Member)res.Value;
            if (!member.HasPermission(market.GetShop(shopId), PermissionsEnum.Permission.MANAGE_ITEMS))
                return new Response<bool>("member doesn't hava permission to this", 4);
            Market.Shop shop = market.GetShop(shopId);
            Stock s = shop.Stock;
            Item t = s.GetItem(productId);
            if (productName == null)
                productName = t.Name;
            if (price == null)
                price = (int?)t.Price;
            if (category == null)
                category = t.Category;
            if (description == null)
                description = t.Description;
            s.ChangeItemDetails(productId, productName, category, (double)price, description);
            return new Response<bool>(true);
        }

        internal Response<int> EnterGuest()
        {
            return userManager.addGuest();
        }

        internal Response<int> Exit(int userId)
        {
            Response<bool> res = userManager.Exit(userId);
            if (res.HasError)
                return new Response<int>(res.ErrorMsg, 3);
            return new Response<int>(userId);

        }

        internal Response<DataObject.User> GetEmployeeInformation(int userId, int shopId, string username)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<DataObject.User>("can't find user", 4);
            Member member = (Member)res.Value;
            if (!member.HasPermission(market.GetShop(shopId), PermissionsEnum.Permission.GET_EMOLOEE_INFO))
                return new Response<DataObject.User>("member doesn't hava permission to this", 6);
            Response<Users.User> resU = userManager.GetUser(username);
            if (resU.HasError)
                return new Response<DataObject.User>(resU.ErrorMsg, 5);
            Users.User u = resU.Value;
            if (!(u is Member))
                return new Response<DataObject.User>("must be member", 5);
            Member m = (Member)u;
            if (!market.GetShop(shopId).IsEmployeeInShop(m))
                return new Response<DataObject.User>("user is not employee in this shop", 5);
            return new Response<DataObject.User>(new DataObject.User(u.SessionId, u.Name, m.Phone, m.Email,
                m.GetPermissions(market.GetShop(shopId))));

        }

        internal Response<int> BuildDiscountPredicate(int memberId, int shopId, string type, int firstParam,
            int? secondParam = 0)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<int>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                var res = market.GetShop(shopId);
                return res.BuildDiscountPredicate(member, type, firstParam, secondParam);
            }
            catch (Exception e)
            {
                return new Response<int>(e.Message, 1);
            }
        }

        internal Response<int> AddDiscountPolicyNumeric(int memberId, int shopId, string description, string type,
            List<int> policies)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<int>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                var res = market.GetShop(shopId);
                return res.AddDiscountPolicyNumeric(member, description, type, policies);
            }
            catch (Exception e)
            {
                return new Response<int>(e.Message, 1);
            }
        }

        internal Response<int> AddDiscountPolicyConditional(int memberId, int shopId, string description, int policyId,
            int predicateId)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<int>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                var res = market.GetShop(shopId);
                return res.AddDiscountPolicyConditional(member, description, policyId, predicateId);
            }
            catch (Exception e)
            {
                return new Response<int>(e.Message, 1);
            }
        }

        internal Response<int> AddDiscountPolicyLogical(int memberId, int shopId, string description, string type,
            int policyId, int predicateId)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<int>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                var res = market.GetShop(shopId);
                return res.AddDiscountPolicyLogical(member, description, type, policyId, predicateId);
            }
            catch (Exception e)
            {
                return new Response<int>(e.Message, 1);
            }
        }

        internal Response<int> AddDiscountPolicyBasic(int memberId, int shopId, string description, double discount,
            string type,
            int? firstParam = 0, string? secondParam = "")
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<int>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                Shop res = market.GetShop(shopId);
                return res.AddDiscountPolicyBasic(member, description, discount, type, firstParam, secondParam);
            }
            catch (Exception e)
            {
                return new Response<int>(e.Message, 1);
            }
        }
        internal Response<bool> ApplyDiscountPolicy(int memberId, int shopId, int policyid)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<bool>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                Shop res = market.GetShop(shopId);
                return res.ApplyDiscountPolicy(member,policyid);
            }
            catch (Exception e)
            {
                return new Response<bool>(e.Message, 1);
            }
        }

        internal Response<int> AddSalePolicyRestriction(int memberId, int shopId, string description,
            string? type = "", string? appliesOn = "Shop", int? firstParam = 0, int? secondParam = 0,
            int? thirdParam = 0, string? forthParam = "",int? fifthParam = 0)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<int>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                Shop res = market.GetShop(shopId);
                return res.AddSalePolicyRestriction(member, description, type, appliesOn, firstParam, secondParam,
                    thirdParam, forthParam, fifthParam);
            }
            catch (Exception e)
            {
                return new Response<int>(e.Message, 1);
            }
        }
        internal Response<bool> ApllySalePolicy(int memberId, int shopId, int policyid)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<bool>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                Shop res = market.GetShop(shopId);
                return res.ApplySalePolicy(member,policyid);
            }
            catch (Exception e)
            {
                return new Response<bool>(e.Message, 1);
            }
        }

        internal Response<int> AddSalePolicyLogical(int memberId, int shopId, string description, string? type = "",
            int? firstParam = 0, int? secondParam = 0)
        {
            try
            {
                Response<User> m = userManager.GetUser(memberId);
                if (m.HasError)
                    return new Response<int>(m.ErrorMsg, 1);
                Member member = (Member)m.Value;
                Shop res = market.GetShop(shopId);
                return res.AddSalePolicyLogical(member, description, type, firstParam, secondParam);
            }
            catch (Exception e)
            {
                return new Response<int>(e.Message, 1);
            }
        }

        internal Response<Product> GetProductInfo(int productId)
        {
            Item t = market.GetProduct(productId);
            if (t is null)
                return new Response<Product>("product not found", 4);
            return new Response<Product>(new Product(t.ItemId, t.Name, t.Price, t.Category, t.Description, t.ShopName));
        }


        internal Response<DataObject.Shop> GetShopInfo(int shopId)
        {
            Market.Shop s = market.GetShop(shopId);
            return new Response<DataObject.Shop>(new DataObject.Shop(s.ShopId, s.Name, s.ShopAddress, s.IsClosed));
        }

        internal Response<DataObject.Shop> GetShopInfo(String shopName)
        {
            Market.Shop s = market.GetShop(shopName);
            return new Response<DataObject.Shop>(new DataObject.Shop(s.ShopId, s.Name, s.ShopAddress, s.IsClosed));
        }

        // shop manager
        internal Response<OrdersHistory> GetShopPurchaseHistory(int userId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<OrdersHistory>("can't find user", 4);
            Member member = (Member)res.Value;
            if (!member.HasPermission(market.GetShop(shopId), PermissionsEnum.Permission.GET_HISTORY_ORDERS))
                return new Response<OrdersHistory>("member doesn't hava permission to this", 6);
            return market.GetShopOrderHistory(shopId);
        }

        public static bool InTestMode;
        //use for reset all system
        internal Response<bool> InitTheSystem(bool inTestMode = true)
        {
            InTestMode = inTestMode;
            //if (inTestMode)
            //    MarketContext.Instance.ResetDB();
            market.InitSystem();
            userManager.InitSystem();
            return new Response<bool>(true);
        }

        internal Response<int> Login(string username, string password, string connectionId)
        {
            return userManager.Login(username, password, connectionId);
        }

        internal Response<bool> Logout(int userId)
        {
            return userManager.Logout(userId);
        }

        internal Response<int> makePayment(string shop_bank, PaymentDetails customer_payment, double amount)
        {
            Response<User> user = userManager.GetUser(customer_payment.Holder);
            return market.MakePayment(shop_bank, customer_payment, amount, user.Value);
        }

        internal Response<DataObject.OrderRecords.Order> MakePurchase(int userId, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError)
                return new Response<DataObject.OrderRecords.Order>("can't find user", 4);
            Users.User u = res.Value;
            Response<Market.Order> res2 = market.ParchaseCart(u.Name, paymentDetails, deliveryDetails);
            if (res2.HasError)
                return new Response<DataObject.OrderRecords.Order>(res2.ErrorMsg, 4);
            u.ShoppingCart.RemoveBaskets();
            return new Response<DataObject.OrderRecords.Order>(new DataObject.OrderRecords.Order(res2.Value));
        }

        internal Response<DataObject.Shop> OpenShop(int userId, string shopName, string shopAddress, string bank)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<DataObject.Shop>("can't add manager", 4);
            Member founder = (Member)res.Value;
            market.AddShop(shopName, founder, shopAddress, bank);
            Shop s = market.GetShop(shopName);
            return new Response<DataObject.Shop>(new DataObject.Shop(s.ShopId, shopName, shopAddress, s.IsClosed));
        }

        internal Response<bool> Register(string username, string password, string email, string address, string phone,
            DateTime birthDate)
        {
            var v = userManager.Register(username, password, email, address, phone, birthDate);
            if (v.HasError)
                return new Response<bool>(v.ErrorMsg, 3);
            return new Response<bool>(true);
        }

        internal Response<bool> RemovePermission(int userId, int shopId, string username,
            PermissionsEnum.Permission permission)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't find user loggedin", 4);
            Member appointer = (Member)res.Value;
            Response<Users.User> res2 = userManager.GetUser(username);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<bool>("can't find user to remove per", 4);
            Member memberToAdd = (Member)res2.Value;
            Market.Shop shop = market.GetShop(shopId);
            if (!appointer.HasPermission(shop, PermissionsEnum.Permission.MANAGE_PERMISSION))
                return new Response<bool>("ONLY owers can remove permmision", 6);
            if (!shop.CheckAppointeeManager(appointer, username))
            {
                return new Response<bool>("ONLY Appointee owner can remove permmision", 4);
            }

            return new Response<bool>(memberToAdd.RemovePermissions(shop, permission));
        }

        internal Response<bool> CanCloseShop(int userId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't find user loggedin", 4);
            Member appointer = (Member)res.Value;
            Market.Shop shop = market.GetShop(shopId);
            if (!appointer.HasPermission(shop, PermissionsEnum.Permission.CLOSE_SHOP))
                return new Response<bool>(false);
            return new Response<bool>(true);
        }
        internal Response<bool> CanOpenShop(int userId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't find user loggedin", 4);
            Member appointer = (Member)res.Value;
            Market.Shop shop = market.GetShop(shopId);
            if (!appointer.HasPermission(shop, PermissionsEnum.Permission.OPEN_SHOP))
                return new Response<bool>(false);
            return new Response<bool>(true);
        }

        internal Response<int> RemoveProductFromCart(int userId, int shopId, int productId, int amountToRemove)
        {
            Response<Users.User> u = userManager.GetUser(userId);
            if (u.HasError)
                return new Response<int>(u.ErrorMsg, 3);
            Market.ShoppingCart cart = u.Value.ShoppingCart;
            Market.Shop shop = market.GetShop(shopId);
            Market.Item item = shop.Stock.GetItem(productId);
            return cart.RemoveItemFromCart(shop.Name, item, amountToRemove);
        }

        internal Response<bool> HasItemInCart(int userId,string shopname, int productid)
        {
            Response<Users.User> u = userManager.GetUser(userId);
            if (u.HasError)
                return new Response<bool>(u.ErrorMsg, 3);
            Market.ShoppingCart cart = u.Value.ShoppingCart;
            Market.Shop shop = market.GetShop(shopname);
            Market.Item item = shop.Stock.GetItem(productid);
            return new Response<bool>(cart.isExist(shopname, item));
        }

        internal Response<bool> EditCart(int userId, int shopId, int productId, int amount)
        {
            Response<Users.User> u = userManager.GetUser(userId);
            if (u.HasError)
                return new Response<bool>(u.ErrorMsg, 3);
            Market.ShoppingCart cart = u.Value.ShoppingCart;
            Market.Shop shop = market.GetShop(shopId);
            Market.Item item = shop.Stock.GetItem(productId);
            return cart.EditItemAmount(shop.Name,item,amount);
        }

        internal Response<bool> RemoveProductFromShop(int userId, int shopId, int productId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't find user", 4);
            Member member = (Member)res.Value;
            if (!member.HasPermission(market.GetShop(shopId), PermissionsEnum.Permission.MANAGE_ITEMS))
                return new Response<bool>("member doesn't hava permission to this", 6);
            Market.Shop s = market.GetShop(shopId);
            string productName = s.Stock.GetItem(productId).Name;
            s.Stock.RemoveItem(productName);
            return new Response<bool>(true);
        }

        internal Response<List<Product>> SearchProducts(string? productName, int? productId, int? minPrice,
            int? maxPrice, string? category)
        {
            if (minPrice == null)
                minPrice = 0;
            if (maxPrice == null)
                maxPrice = int.MaxValue;
            List<Item> items = market.SearchProducts(productName, productId, minPrice, maxPrice, category);
            List<Product> products = new List<Product>();
            foreach (Item i in items)
            {
                products.Add(new Product(i.ItemId, i.Name, i.Price, i.Category, i.Description, i.ShopName));
            }

            return new Response<List<Product>>(products);

        }

        internal Response<DataObject.ShoppingCart> ViewShoppingCart(int userId)
        {
            Response<Users.User> u = userManager.GetUser(userId);
            if (u.HasError)
                return new Response<DataObject.ShoppingCart>(u.ErrorMsg, 3);
            Market.ShoppingCart cart = u.Value.ShoppingCart;
            return new Response<DataObject.ShoppingCart>(new DataObject.ShoppingCart(cart));
        }

        public Response<bool> RemoveAppointOwner(int userId, int shopId, string username)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't find user by userId", 4);
            Member owner = (Member)res.Value;
            Response<Users.User> res2 = userManager.GetUser(username);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<bool>("can't find user by username", 4);
            Member ownerToRemove = (Member)res2.Value;
            Market.Shop shop = market.GetShop(shopId);
            return new Response<bool>(shop.RemoveAppointOwner(owner, ownerToRemove));
        }

        public Response<bool> RemoveAppointManager(int userId, int shopId, string username)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(res.Value is Member))
                return new Response<bool>("can't find user by userId", 4);
            Member owner = (Member)res.Value;
            Response<Users.User> res2 = userManager.GetUser(username);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<bool>("can't find user by username", 4);
            Member managerToRemove = (Member)res2.Value;
            Market.Shop shop = market.GetShop(shopId);
            return new Response<bool>(shop.RemoveAppointManager(owner, managerToRemove));
        }

        //system manager:

        /// <summary>
        /// PurchaseHistoryOfShop for System manager
        /// </summary>
        /// <param name="userId">system manager</param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        internal Response<List<DataObject.OrderRecords.PastOrder>> GetPurchaseHistoryOfShop(int userId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(userManager.IsSystemManager(res.Value)))
                return new Response<List<DataObject.OrderRecords.PastOrder>>(
                    "can't find user or he isn't SystemManager", 4);
            Response<OrdersHistory> res2 = market.GetShopOrderHistory(shopId);
            if (res2.HasError)
                return new Response<List<DataObject.OrderRecords.PastOrder>>(res2.ErrorMsg, 4);
            OrdersHistory oh = res2.Value;
            List<DataObject.OrderRecords.PastOrder> orders = oh.Orders;
            return new Response<List<DataObject.OrderRecords.PastOrder>>(orders);

        }

        /// <summary>
        /// PurchaseHistoryOfUser for System manager
        /// </summary>
        /// <param name="userId">System manager</param>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal Response<List<DataObject.OrderRecords.PastUserOrder>> GetPurchaseHistoryOfUser(int userId,
            string username)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(userManager.IsSystemManager(res.Value)))
                return new Response<List<DataObject.OrderRecords.PastUserOrder>>(
                    "can't find user or he doesn't system manager", 4);
            Response<Users.User> res2 = userManager.GetUser(username);
            if (res2.HasError || !(res.Value is Member))
                return new Response<List<DataObject.OrderRecords.PastUserOrder>>("can't find user by username", 4);
            Member user = (Member)res2.Value;
            return new Response<List<DataObject.OrderRecords.PastUserOrder>>(user.PastOrders);
        }


        public Response<bool> CloseShopForever(int userId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(userManager.IsSystemManager(res.Value)))
                return new Response<bool>("can't find user or he doesn't system manager", 4);
            Market.Shop shop = market.GetShop(shopId);
            return market.CloseShopForever(shop);
        }

        public Response<User> GetUser(int userid)
        {
            return userManager.GetUser(userid);
        }

        public Response<User> GetUser(string userName)
        {
            return userManager.GetUser(userName);
        }

        public Response<bool> RemoveMember(int userId, string username)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError || !(userManager.IsSystemManager(res.Value)))
                return new Response<bool>("can't find user or he doesn't system manager", 4);
            Response<Users.User> res2 = userManager.GetUser(username);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<bool>("can't find user by username", 4);
            Member memberToRemove = (Member)res2.Value;
            return userManager.RemoveMember(memberToRemove);
        }

        public Response<List<DataObject.Shop>> getUsersShop(int userId)
        {
            Response<Users.User> res2 = userManager.GetUser(userId);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<List<DataObject.Shop>>("can't find user by userId", 4);
            Member mem = (Member)res2.Value;
            List<DataObject.Shop> shops = userManager.GetUserShop(mem);
            if (shops != null)
            {
                return new Response<List<DataObject.Shop>>(shops);
            }
            return new Response<List<DataObject.Shop>>("error in get user shops", 4);
        }

        public Response<List<DataObject.Shop>> GetUserShopsToOpen(int userId)
        {
            Response<Users.User> res2 = userManager.GetUser(userId);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<List<DataObject.Shop>>("can't find user by userId", 4);
            Member mem = (Member)res2.Value;
            List<DataObject.Shop> shops = userManager.GetUserShopsToOpen(mem);
            if (shops != null)
            {
                return new Response<List<DataObject.Shop>>(shops);
            }
            return new Response<List<DataObject.Shop>>("error in get user shops", 4);
        }

        public Response<List<DataObject.Shop>> GetUserOpenShops(int userId)
        {
            Response<Users.User> res2 = userManager.GetUser(userId);
            if (res2.HasError || !(res2.Value is Member))
                return new Response<List<DataObject.Shop>>("can't find user by userId", 4);
            Member mem = (Member)res2.Value;
            List<DataObject.Shop> shops = userManager.GetUserOpenShops(mem);
            if (shops != null)
            {
                return new Response<List<DataObject.Shop>>(shops);
            }
            return new Response<List<DataObject.Shop>>("error in get user shops", 4);
        }

        public Response<List<(Product,int)>> GetShopProducts(int shopid)
        {
            List<(Product,int)> products = market.GetShopProducts(shopid);
            return new Response<List<(Product, int)>>(products);
        }

        //GetShopProductsWithoutQuantity
        public Response<List<Product>> GetShopProductsWithoutQuantity(int shopid)
        {
            List<Product> products = market.GetShopProductsWithoutQuantity(shopid);
            return new Response<List<Product>>(products);
        }

        public Member GetDomainUser(string username)
        {
            Response<Users.User> res = userManager.GetUser(username);
            if (res.HasError || !(res.Value is Member))
                return null;
            return (Member)res.Value;
        }

        internal Response<string> GetDiscountPolicies(int memberId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(memberId);
            if (res.HasError || !(res.Value is Member))
                return new Response<string>("can't find user", 4);
            Member member = (Member)res.Value;
            if (!member.HasPermission(market.GetShop(shopId), PermissionsEnum.Permission.MANAGE_POLICIES))
                return new Response<string>("member doesn't hava permission to this", 6);
            Market.Shop s = market.GetShop(shopId);
            return new Response<string>(s.GetDiscountPolicyManager());
        }

        internal Response<string> GetSalePolicies(int memberId, int shopId)
        {
            Response<Users.User> res = userManager.GetUser(memberId);
            if (res.HasError || !(res.Value is Member))
                return new Response<string>("can't find user", 4);
            Member member = (Member)res.Value;
            if (!member.HasPermission(market.GetShop(shopId), PermissionsEnum.Permission.MANAGE_POLICIES))
                return new Response<string>("member doesn't hava permission to this", 6);
            Market.Shop s = market.GetShop(shopId);
            return new Response<string>(s.GetSalePolicyManager());
        }

        public Response<double> GetCartPrice(int userId)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError)
                return new Response<double>("can't find user", 4);
            Users.User u = res.Value;
            DomainLayer.Market.ShoppingCart cart = u.ShoppingCart;
            Dictionary<string,DiscountPolicyManager> policies = new Dictionary<string,DiscountPolicyManager>();
            IEnumerable<ShopBasket> baskets = cart.GetBaskets();
            foreach (ShopBasket b in baskets)
            {
                DiscountPolicyManager discountManager = market.GetShop(b.ShopName).discountPolicy;
                policies[b.ShopName] = discountManager;
            }
            return new Response<double>(cart.GetPrice(policies));
        }

        public Response<double> GetBasketPrice(int userId, string shopName)
        {
            Response<Users.User> res = userManager.GetUser(userId);
            if (res.HasError)
                return new Response<double>("can't find user", 4);
            Users.User u = res.Value;
            DomainLayer.Market.ShoppingCart cart = u.ShoppingCart;
            ShopBasket basket = cart.GetBasket(shopName);
            if (basket is null)
                return new Response<double>("can't find basket", 4);
            return new Response<double>(basket.GetPrice(market.GetShop(shopName).discountPolicy));
        }
    }
}
