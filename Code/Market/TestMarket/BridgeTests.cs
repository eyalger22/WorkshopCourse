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


namespace TestMarket
{
    internal interface BridgeTests
    {
        /// <summary>
        /// 1. System
        /// </summary>
        
        //1.1
        Response<bool> InitTheSystem();

        //1.3 Payment
        Response<int> makePayment(string shop_bank, PaymentDetails customer_payment, double amount);
        //1.4 Delivery
        Response<int> createDelivery(string name, string address, string city, string country, string zip);


        /// <summary>
        /// 2.1. Guest
        /// </summary>
        //2.1.1 Enter
        Response<int> EnterGuest();

        //2.1.2 Exit
        Response<int> Exit(int userId);
        //2.1.3 Register
        Response<bool> Register(string username, string password, string email, string address, string phone = "");
        //2.1.4 Login
        Response<int> Login(string username, string password);

        /// <summary>
        /// 2.2. Guest buying
        /// </summary>
        //2.2.1 – get shop/product information
        Response<Shop> GetShopInfo(int shopId);
        Response<Product> GetProductInfo(int productId);
        //2.2.2 – search for products
        Response<List<Product>> SearchProducts(string? productName = null, int? productId = null, int? minPrice = null, int? maxPrice = null);
        //2.2.3 – add/remove products to/from shopping cart
        Response<bool> AddProductToCart(int userId, int shopId, int productId, int amount);
        //2.2.4 – view shopping cart and edit
        Response<ShoppingCart> ViewShoppingCart(int userId);
        Response<int> RemoveProductFromCart(int userId, int shopId, int productId, int amountToRemove); // return new amount
        //2.2.5 – purchase products
        Response<Order> MakePurchase(int userId, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails);


        /// <summary>
        /// 2.3. Member
        /// </summary>
        //2.3.1 logout
        Response<bool> Logout(int userId);
        //2.3.2 OpenNewShop
        Response<Shop> OpenShop(int userId, string shopName, string shopAddress, string bank);


        /// <summary>
        /// 2.4 Shop Owner
        /// </summary>
        //2.4.1.1 AddProductToShop
        Response<int> AddProductToShop(int userId, int shopId, string productName, int price, string category, string description);
        Response<int> AddAmountToProductInStock(int userId, int shopId, int productId, int amount); //return new amount
        //
        //2.4.1.2 RemoveProductFromShop
        Response<bool> RemoveProductFromShop(int userId, int shopId, int productId);
        //2.4.1.3 EditProductDetails
        Response<bool> EditProductDetails(int userId, int shopId, int productId, string? productName = null, int? price = null, string? category = null, string? description = null);
        //2.4.4 Add owner
        Response<bool> AddOwner(int userId, int shopId, string usernameNewOwner);
        //2.4.5 Remove owner - not needed for this version
        //Response<bool> RemoveOwner(int userId, int shopId, string username);
        //2.4.6 Add manager
        Response<bool> AddManager(int userId, int shopId, string username);
        
        //2.4.7 Add permission
        Response<string> AddPermission(int userId, int shopId, string username, PermissionsEnum.Permission permission); //return user Permission
        Response<bool> RemovePermission(int userId, int shopId, string username, PermissionsEnum.Permission permission);
        //2.4.8 Remove manager - not needed for this version
        //Response<bool> RemoveManager(int userId, int shopId, string username);

        //2.4.9 Close shop
        Response<int> CloseShop(int userId, int shopId);
        //2.4.11 Get employee information
        Response<User> GetEmployeeInformation(int userId, int shopId, string username);
        //2.4.13 Get shop purchase history
        Response<OrdersHistory> GetShopPurchaseHistory(int userId, int shopId);
        
        
        /// <summary>
        /// 2.5 Shop Manager
        /// </summary>

        /// <summary>
        /// 2.6 System Manager
        /// </summary>
        
        //2.6.4 Get shop purchase history of user
        Response<List<Market.DataObject.OrderRecords.PastUserOrder>> GetPurchaseHistoryOfUser(int userId, string username);
        //2.6.4 Get shop purchase history of shop
        Response<List<Market.DataObject.OrderRecords.PastOrder>> GetPurchaseHistoryOfShop(int userId, int shopId);


        Response<bool> RemoveOwner(int userId, int shopId,string username);
        Response<bool> Unregister(int userId, string username);

        public Response<List<string>> ReadAlerts(int userId);


        /// <summary>
        /// Discount Policy 
        /// </summary>

        public Response<int> BuildDiscountPredicate(int memberId, int shopId, string predicateType, int firstParam,
            int? secondParam = 0);

        public Response<int> AddDiscountPolicyNumeric(int memberId, int shopId, string description, string type,
            List<int> policies);

        public Response<int> AddDiscountPolicyConditional(int memberId, int shopId, string description, int policyId,
            int predicateId);

        public Response<int> AddDiscountPolicyLogical(int memberId, int shopId, string description, string type,
            int policyId, int predicateId);

        public Response<int> AddDiscountPolicyBasic(int memberId, int shopId, string description, double discount,
            string type,
            int? firstParam = 0, string? secondParam = "");

        public Response<string> GetDiscountPolicies(int memberId, int shopId);
        
        public Response<bool> ApplyDiscountPolicy(int memberId, int shopId, int policyid);

        /// <summary>
        /// Sale Policy 
        /// </summary>
        public Response<int> AddSalePolicyRestriction(int memberId, int shopId, string description,
            string? type = "", string? appliesOn = "Shop", int? firstParam = 0, int? secondParam = 0,
            int? thirdParam = 0, string? forthParam = "", int? fifthParam = 0);

        public Response<int> AddSalePolicyLogical(int memberId, int shopId, string description, string? type = "",
            int? firstParam = 0, int? secondParam = 0);

        public Response<string> GetSalePolicies(int memberId, int shopId);
        
        
    }
}
