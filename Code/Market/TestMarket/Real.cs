using Market.DataObject;
using Market.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Order = Market.DataObject.OrderRecords.Order;
using Shop = Market.DataObject.Shop;
using OrdersHistory = Market.DataObject.OrderRecords.OrdersHistory;


namespace TestMarket
{
    internal class Real : BridgeTests
    {
        private Service service;
        public Real(Service s)
        {
            service = s;
        }
        public Response<int> AddAmountToProductInStock(int userId, int shopId, int productId, int amount)
        {
            return service.ShopService.AddAmountToProductInStock(userId, shopId, productId, amount);
        }

        public Response<bool> AddManager(int userId, int shopId, string username)
        {
            return service.ShopService.AddManager(userId, shopId, username);
        }

        public Response<bool> AddOwner(int userId, int shopId, string username)
        {
            return service.ShopService.AddOwner(userId, shopId, username);
        }

        public Response<string> AddPermission(int userId, int shopId, string username, PermissionsEnum.Permission permission)
        {
            return service.ShopService.AddPermission(userId, shopId, username, permission);
        }

        public Response<bool> AddProductToCart(int userId, int shopId, int productId, int amount)
        {
            return service.UserService.AddProductToCart(userId, shopId, productId, amount);
        }

        public Response<int> AddProductToShop(int userId, int shopId, string productName, int price, string category, string description)
        {
            return service.ShopService.AddProductToShop(userId, shopId, productName, price, category, description);
        }

        public Response<int> CloseShop(int userId, int shopId)
        {
            return service.ShopService.CloseShop(userId, shopId);
        }

        public Response<int> createDelivery(string name, string address, string city, string country, string zip)
        {
            return service.ShopService.createDelivery(name, address, city, country, zip);
        }

        public Response<bool> EditProductDetails(int userId, int shopId, int productId, string? productName = null, int? price = null, string? category = null, string? description = null)
        {
            return service.ShopService.EditProductDetails(userId, shopId, productId, productName, price, category, description);
        }

        public Response<int> EnterGuest()
        {
            return service.UserService.EnterGuest();
        }

        public Response<int> Exit(int userId)
        {
            return service.UserService.Exit(userId);
        }

        public Response<Market.DataObject.User> GetEmployeeInformation(int userId, int shopId, string username)
        {
            return service.UserService.GetEmployeeInformation(userId, shopId, username);
        }

        public Response<Product> GetProductInfo(int productId)
        {
            return service.ShopService.GetProductInfo(productId);
        }

        public Response<List<Market.DataObject.OrderRecords.PastOrder>> GetPurchaseHistoryOfShop(int userId, int shopId)
        {
            return service.ShopService.GetPurchaseHistoryOfShop(userId, shopId);
        }

        public Response<List<Market.DataObject.OrderRecords.PastUserOrder>> GetPurchaseHistoryOfUser(int userId, string username)
        {
            return service.ShopService.GetPurchaseHistoryOfUser(userId, username);
        }

        public Response<Shop> GetShopInfo(int shopId)
        {
            return service.ShopService.GetShopInfo(shopId);
        }

        public Response<OrdersHistory> GetShopPurchaseHistory(int userId, int shopId)
        {
            return service.ShopService.GetShopPurchaseHistory(userId, shopId);
        }

        public Response<bool> InitTheSystem()
        {
            return service.UserService.InitTheSystem();
        }

        public Response<int> Login(string username, string password)
        {
            return service.UserService.Login(username, password);
        }

        public Response<bool> Logout(int userId)
        {
            return service.UserService.Logout(userId);
        }

        public Response<int> makePayment(string shop_bank, PaymentDetails customer_payment, double amount)
        {
            return service.ShopService.makePayment(shop_bank, customer_payment, amount);
        }

        public Response<Order> MakePurchase(int userId, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            return service.ShopService.MakePurchase(userId, paymentDetails, deliveryDetails);
        }

        public Response<Shop> OpenShop(int userId, string shopName, string shopAddress, string bank)
        {
            return service.ShopService.OpenShop(userId, shopName, shopAddress, bank);
        }

        public Response<bool> Register(string username, string password, string email, string address, string phone = "")
        {
            return service.UserService.Register(username, password, email, address, phone);
        }

        public Response<bool> RemovePermission(int userId, int shopId, string username, PermissionsEnum.Permission permission)
        {
            return service.UserService.RemovePermission(userId, shopId, username, permission);
        }

        public Response<int> RemoveProductFromCart(int userId,int shopId ,int productId, int amountToRemove)
        {
            return service.ShopService.RemoveProductFromCart(userId, shopId, productId, amountToRemove);
        }

        public Response<bool> RemoveProductFromShop(int userId, int shopId, int productId)
        {
            return service.ShopService.RemoveProductFromShop(userId, shopId, productId);
        }

        public Response<List<Product>> SearchProducts(string? productName = null, int? productId = null, int? minPrice = null, int? maxPrice = null)
        {
            return service.ShopService.SearchProducts(productName, productId, minPrice, maxPrice);
        }

        public Response<Market.DataObject.ShoppingCart> ViewShoppingCart(int userId)
        {
            return service.UserService.ViewShoppingCart(userId);
        }

        public Response<bool> RemoveOwner(int userId, int shopId, string username)
        {
            return service.UserService.RemoveAppointManager(userId, shopId, username);
        }

        public Response<bool> Unregister(int userId, string username)
        {
            return service.UserService.RemoveMember(userId, username);
        }

        public Response<List<string>> ReadAlerts(int userId)
        {
            return service.UserService.ReadAlert(userId);
        }

        public Response<int> BuildDiscountPredicate(int memberId, int shopId, string predicateType, int firstParam, int? secondParam = 0)
        {
            return service.ShopService.BuildDiscountPredicate(memberId, shopId, predicateType, firstParam, secondParam);
        }

        public Response<int> AddDiscountPolicyNumeric(int memberId, int shopId, string description, string type, List<int> policies)
        {
            return service.ShopService.AddDiscountPolicyNumeric(memberId, shopId, description, type, policies);
        }

        public Response<int> AddDiscountPolicyConditional(int memberId, int shopId, string description, int policyId, int predicateId)
        {
            return service.ShopService.AddDiscountPolicyConditional(memberId, shopId, description, policyId, predicateId);
        }

        public Response<int> AddDiscountPolicyLogical(int memberId, int shopId, string description, string type, int policyId,
            int predicateId)
        {
            return service.ShopService.AddDiscountPolicyLogical(memberId, shopId, description, type, policyId, predicateId);
        }

        public Response<int> AddDiscountPolicyBasic(int memberId, int shopId, string description, double discount, string type,
            int? firstParam, string? secondParam = "")
        {
            return service.ShopService.AddDiscountPolicyBasic(memberId, shopId, description, discount, type, firstParam, secondParam);
        }

        public Response<string> GetDiscountPolicies(int memberId, int shopId)
        {
            return service.ShopService.GetDiscountPolicies(memberId, shopId);
        }

        public Response<int> AddSalePolicyRestriction(int memberId, int shopId, string description, string? type = "",
            string? appliesOn = "Shop", int? firstParam = 0, int? secondParam = 0, int? thirdParam = 0, string? forthParam = "", int? fifthParam =0)
        {
            return service.ShopService.AddSalePolicyRestriction(memberId, shopId, description, type, appliesOn, firstParam, secondParam, thirdParam, forthParam, fifthParam);
        }

        public Response<int> AddSalePolicyLogical(int memberId, int shopId, string description, string? type = "", int? firstParam = 0,
            int? secondParam = 0)
        {
            return service.ShopService.AddSalePolicyLogical(memberId, shopId, description, type, firstParam, secondParam);
        }

        public Response<string> GetSalePolicies(int memberId, int shopId)
        {
            return service.ShopService.GetSalePolicies(memberId, shopId);
        }

        public Response<bool> ApplyDiscountPolicy(int memberId, int shopId, int policyid)
        {
            return service.ShopService.ApplyDiscountPolicy(memberId, shopId, policyid);
        }
    }
}
