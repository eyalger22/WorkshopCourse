using Market.DataObject;
//using Market.DomainLayer.Market;
//using Market.DomainLayer.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = Market.DataObject.OrderRecords.Order;
using Shop = Market.DataObject.Shop;
using ShoppingCart = Market.DataObject.ShoppingCart;
using OrdersHistory = Market.DataObject.OrderRecords.OrdersHistory;
using Market.DomainLayer.Market;


namespace TestMarket
{
    internal class Proxy : BridgeTests
    {
        public Real? r { get;  set; }

        public Proxy()
        {
            r = null;
        }
        
       

        public Response<bool> AddManager(int userId, int shopId, string username)
        {
            if (r != null)
            {
                return r.AddManager(userId, shopId, username);
            }
            return new Response<bool>("Proxy: real not set", 0);
        }


        public Response<bool> AddOwner(int userId, int shopId, string usernameNewOwner)
        {
            if (r != null)
            {
                return r.AddOwner(userId, shopId, usernameNewOwner);
            }
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<string> AddPermission(int userId, int shopId, string username, PermissionsEnum.Permission permission)
        {
            if (r != null)
                return r.AddPermission(userId, shopId, username, permission);
            return new Response<string>("Proxy: real not set", 0);
        }

        public Response<bool> AddProductToCart(int userId, int shopId, int productId, int amount)
        {
            if (r != null)
                return r.AddProductToCart(userId,shopId, productId, amount);
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<int> AddProductToShop(int userId, int shopId, string productName, int price, string category, string description)
        {
            if (r != null)
                return r.AddProductToShop(userId, shopId, productName, price, category, description);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<int> CloseShop(int userId, int shopId)
        {
            if (r != null)
                return r.CloseShop(userId, shopId);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<int> createDelivery(string name, string address, string city, string country, string zip)
        {
            if (r != null)
                return r.createDelivery(name, address, city, country, zip);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<bool> EditProductDetails(int userId, int shopId, int productId, string? productName = null, int? price = null, string? category = null, string? description = null)
        {
            if (r != null)
                return r.EditProductDetails(userId, shopId, productId, productName, price, category, description);
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<int> EnterGuest()
        {
            if (r != null)
                return r.EnterGuest();
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<int> Exit(int userId)
        {
            if (r != null)
                return r.Exit(userId);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<User> GetEmployeeInformation(int userId, int shopId, string username)
        {
            if (r != null)
                return r.GetEmployeeInformation(userId, shopId, username);
            return new Response<User>("Proxy: real not set", 0);
        }

        public Response<Product> GetProductInfo(int productId)
        {
            if (r != null)
                return r.GetProductInfo(productId);
            return new Response<Product>("Proxy: real not set", 0);
        }

        public Response<List<Market.DataObject.OrderRecords.PastOrder>> GetPurchaseHistoryOfShop(int userId, int shopId)
        {
            if (r != null)
                return r.GetPurchaseHistoryOfShop(userId, shopId);
            return new Response<List<Market.DataObject.OrderRecords.PastOrder>>("Proxy: real not set", 0);
        }

        public Response<List<Market.DataObject.OrderRecords.PastUserOrder>> GetPurchaseHistoryOfUser(int userId, string username)
        {
            if (r != null)
                return r.GetPurchaseHistoryOfUser(userId, username);
            return new Response<List<Market.DataObject.OrderRecords.PastUserOrder>>("Proxy: real not set", 0);
        }

        public Response<Shop> GetShopInfo(int shopId)
        {
            if (r != null)
                return r.GetShopInfo(shopId);
            return new Response<Shop>("Proxy: real not set", 0);
        }

        public Response<OrdersHistory> GetShopPurchaseHistory(int userId, int shopId)
        {
            if (r != null)
                return r.GetShopPurchaseHistory(userId, shopId);
            return new Response<OrdersHistory>("Proxy: real not set", 0);
        }

        public Response<bool> InitTheSystem()
        {
            if (r != null)
                return r.InitTheSystem();
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<int> Login(string username, string password)
        {
            if (r != null)
                return r.Login(username, password);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<bool> Logout(int userId)
        {
            if (r != null)
                return r.Logout(userId);
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<int> makePayment(string shop_bank, PaymentDetails customer_payment, double amount)
        {
            if (r != null)
                return r.makePayment(shop_bank, customer_payment, amount);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<Order> MakePurchase(int userId, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            if (r != null)
                return r.MakePurchase(userId, paymentDetails, deliveryDetails);
            return new Response<Order>("Proxy: real not set", 0);
        }

        public Response<Shop> OpenShop(int userId, string shopName, string shopAddress, string bank)
        {
            if (r != null)
                return r.OpenShop(userId, shopName, shopAddress, bank);
            return new Response<Shop>("Proxy: real not set", 0);
        }

        public Response<bool> Register(string username, string password, string email, string address, string phone = "")
        {
            if (r != null)
                return r.Register(username, password, email, address, phone);
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<bool> RemovePermission(int userId, int shopId, string username, PermissionsEnum.Permission permission)
        {
            if (r != null)
                return r.RemovePermission(userId, shopId, username, permission);
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<int> RemoveProductFromCart(int userId, int shopId, int productId, int amountToRemove)
        {
            if (r != null)
                return r.RemoveProductFromCart(userId,shopId, productId, amountToRemove);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<bool> RemoveProductFromShop(int userId, int shopId, int productId)
        {
            if (r != null)
                return r.RemoveProductFromShop(userId, shopId, productId);
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<List<Product>> SearchProducts(string? productName = null, int? productId = null, int? minPrice = null, int? maxPrice = null)
        {
            if (r != null)
                return r.SearchProducts(productName, productId, minPrice, maxPrice);
            return new Response<List<Product>>("Proxy: real not set", 0);
        }

        public Response<ShoppingCart> ViewShoppingCart(int userId)
        {
            if (r != null)
                return r.ViewShoppingCart(userId);
            return new Response<ShoppingCart>("Proxy: real not set", 0);
        }

        public Response<int> AddAmountToProductInStock(int userId, int shopId, int productId, int amount)
        {
            if (r != null)
                return r.AddAmountToProductInStock(userId, shopId, productId, amount);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<bool> RemoveOwner(int userId,int  shopId, string username)
        {
            if (r != null)
                return r.RemoveOwner(userId, shopId, username);
            return new Response<bool>("Proxy: real not set", 0);
        }

        public Response<bool> Unregister(int userId,string username)
        {
            if (r != null)
                return r.Unregister(userId, username);
            return new Response<bool>("Proxy: real not set", 0);

        }

        public Response<List<string>> ReadAlerts(int userId)
        {
            if (r != null)
                return r.ReadAlerts(userId);
            return new Response<List<string>>("Proxy: real not set", 0);
        }

        public Response<int> BuildDiscountPredicate(int memberId, int shopId, string predicateType, int firstParam, int? secondParam = 0)
        {
            if (r != null)
                return r.BuildDiscountPredicate(memberId, shopId, predicateType, firstParam, secondParam);
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<int> AddDiscountPolicyNumeric(int memberId, int shopId, string description, string type, List<int> policies)
        {
            if (r != null)
            {
                return r.AddDiscountPolicyNumeric(memberId, shopId, description, type, policies);
            }
            return new Response<int>("Proxy: real not set", 0);
            
        }

        public Response<int> AddDiscountPolicyConditional(int memberId, int shopId, string description, int policyId, int predicateId)
        {
            if (r != null)
            {
                return r.AddDiscountPolicyConditional(memberId, shopId, description, policyId, predicateId);
            }
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<int> AddDiscountPolicyLogical(int memberId, int shopId, string description, string type, int policyId,
            int predicateId)
        {
            if (r != null)
            {
                return r.AddDiscountPolicyLogical(memberId, shopId, description, type, policyId, predicateId);
            }
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<int> AddDiscountPolicyBasic(int memberId, int shopId, string description, double discount, string type,
            int? firstParam, string? secondParam = "")
        {
            if (r != null)
            {
                return r.AddDiscountPolicyBasic(memberId, shopId, description, discount, type, firstParam, secondParam);
            }
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<string> GetDiscountPolicies(int memberId, int shopId)
        {
            if (r != null)
            {
                return r.GetDiscountPolicies(memberId, shopId);
            }
            return new Response<string>("Proxy: real not set", 0);
        }

        public Response<int> AddSalePolicyRestriction(int memberId, int shopId, string description, string? type = "",
            string? appliesOn = "Shop", int? firstParam = 0, int? secondParam = 0, int? thirdParam = 0, string? forthParam = "",int? fifthParam = 0)
        {
            if (r != null)
            {
                return r.AddSalePolicyRestriction(memberId, shopId, description, type, appliesOn, firstParam, secondParam, thirdParam, forthParam, fifthParam);
            }
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<int> AddSalePolicyLogical(int memberId, int shopId, string description, string? type = "", int? firstParam = 0,
            int? secondParam = 0)
        {
            if (r != null)
            {
                return r.AddSalePolicyLogical(memberId, shopId, description, type, firstParam, secondParam);
            }
            return new Response<int>("Proxy: real not set", 0);
        }

        public Response<string> GetSalePolicies(int memberId, int shopId)
        {
            if (r != null)
            {
                return r.GetSalePolicies(memberId, shopId);
            }
            return new Response<string>("Proxy: real not set", 0);
        }

        public Response<bool> ApplyDiscountPolicy(int memberId, int shopId, int policyid)
        {
            if (r != null)
            {
                return r.ApplyDiscountPolicy(memberId, shopId, policyid);
            }
            return new  Response<bool>("Proxy: real not set", 0);
        }
    }
}
