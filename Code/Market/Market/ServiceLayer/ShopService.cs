using Market.DataObject;
using Facade = Market.DomainLayer.Facade;
using Order = Market.DataObject.OrderRecords.Order;
using Shop = Market.DataObject.Shop;
using ShoppingCart = Market.DataObject.ShoppingCart;
using OrdersHistory = Market.DataObject.OrderRecords.OrdersHistory;
using Serilog;
using System.Collections.Generic;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Market.ServiceLayer
{
    public class ShopService
    {
        private Facade facade;
        public ShopService()
        {
            facade = Facade.GetFacade();
        }

        public Response<int> AddAmountToProductInStock(int userId, int shopId, int productId, int amount)
        {
            try
            {
                Response<int> res = facade.AddAmountToProductInStock(userId, shopId, productId, amount);
                if (res.HasError)
                {
                    Log.Error("AddAmountToProductInStock(userId:"+userId+", shopId:"+shopId+", productId:"+productId+", amount:"+amount+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddAmountToProductInStock(userId:"+userId+", shopId:"+shopId+", productId:"+productId+", amount:"+amount+") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddAmountToProductInStock(userId:" + userId + ", shopId:" + shopId + ", productId:" + productId + ", amount:" + amount + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddAmountToProductInStock: "+ ex.Message, 2);
            }
        }

        public Response<bool> AddManager(int userId, int shopId, string username)
        {
            try
            {
                Response<bool> res = facade.AddManager(userId, shopId, username);
                if (res.HasError)
                {
                    Log.Error("AddManager(userId:"+userId+", shopId:"+shopId+", username:"+username+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddManager(userId:" + userId + ", shopId:" + shopId + ", username:" + username + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddManager(userId:" + userId + ", shopId:" + shopId + ", username:" + username + ") ... Failure: " + ex.Message);
                return new Response<bool>("Error in AddManager: " + ex.Message, 2);
            }
        }

        public Response<bool> AddOwner(int userId, int shopId, string usernameNewOwner)
        {
            try
            {
                Response<bool> res = facade.AddOwner(userId, shopId, usernameNewOwner);
                if (res.HasError)
                {
                    Log.Error("AddOwner(userId:"+userId+", shopId:"+shopId+", usernameNewOwner:"+usernameNewOwner+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddOwner(userId:" + userId + ", shopId:" + shopId + ", usernameNewOwner:" + usernameNewOwner + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddOwner(userId:" + userId + ", shopId:" + shopId + ", usernameNewOwner:" + usernameNewOwner + ") ... Failure: " + ex.Message);
                return new Response<bool>("Error in AddOwner: " + ex.Message, 2);
            }
        }

        public Response<string> AddPermission(int userId, int shopId, string username, PermissionsEnum.Permission permission)
        {
            try
            {
                Response<string> res = facade.AddPermission(userId, shopId, username, permission);
                if (res.HasError)
                {
                    Log.Error("AddPermission(userId:"+userId+", shopId:"+shopId+", username:"+username+", permission:"+permission.ToString()+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddPermission(userId:" + userId + ", shopId:" + shopId + ", username:" + username + ", permission:" + permission.ToString() + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddPermission(userId:" + userId + ", shopId:" + shopId + ", username:" + username + ", permission:" + permission.ToString() + ") ... Failure: " + ex.Message);
                return new Response<string>("Error in AddPermission: " + ex.Message, 2);
            }
        }

        public Response<int> AddProductToShop(int userId, int shopId, string productName, int price, string category, string description)
        {
            try
            {
                Response<int> res = facade.AddProductToShop(userId, shopId, productName, price, category, description);
                if (res.HasError)
                {
                    Log.Error("AddProductToShop(userId:"+userId+", shopId:"+shopId+", productName:"+productName+", price:"+price+", category:"+category+", description:"+description+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddProductToShop(userId:" + userId + ", shopId:" + shopId + ", productName:" + productName + ", price:" + price + ", category:" + category + ", description:" + description + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddProductToShop(userId:" + userId + ", shopId:" + shopId + ", productName:" + productName + ", price:" + price + ", category:" + category + ", description:" + description + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddProductToShop: " + ex.Message, 2);
            }
        }

        public Response<int> CloseShop(int userId, int shopId)
        {
            try
            {
                Response<int> res = facade.CloseShop(userId, shopId);
                if (res.HasError)
                {
                    Log.Error("CloseShop(userId:"+userId+", shopId"+shopId+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("CloseShop(userId:" + userId + ", shopId" + shopId + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("CloseShop(userId:" + userId + ", shopId" + shopId + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in CloseShop: " + ex.Message, 2);
            }
        }

        public Response<int> createDelivery(string name, string address, string city, string country, string zip)
        {
            string str = "CreateDelivery(name: " + name + ", address: " + address + ", city: " + city + ", zip: " + zip + ") ... ";
            try
            {
                Response<int> res = facade.createDelivery(name, address, city, country, zip);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>("Error in createDelivery: " + ex.Message, 2);
            }
        }

        public Response<bool> EditProductDetails(int userId, int shopId, int productId, string? productName = null, int? price = null, string? category = null, string? description = null)
        {
            try
            {
                Response<bool> res = facade.EditProductDetails(userId, shopId, productId, productName, price, category, description);
                if (res.HasError)
                {
                    Log.Error("EditProductDetails(userId:"+userId+", shopId:"+shopId+", productId:"+productId/*+", productName:"+productName+", price:"+price+", category:"+category+"description:"+description*/+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("EditProductDetails(userId:" + userId + ", shopId:" + shopId + ", productId:" + productId/* + ", productName:" + productName + ", price:" + price + ", category:" + category + "description:" + description */+ ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("EditProductDetails(userId:" + userId + ", shopId:" + shopId + ", productId:" + productId/* + ", productName:" + productName + ", price:" + price + ", category:" + category + "description:" + description */+ ") ... Failure: " + ex.Message);
                return new Response<bool>("Error in EditProductDetails: "+ ex.Message, 2);
            }
        }


        public Response<Product> GetProductInfo(int productId)
        {
            try
            {
                Response < Product > res = facade.GetProductInfo(productId);
                if (res.HasError)
                {
                    Log.Error("GetProductInfo(productId:"+productId+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetProductInfo(productId:" + productId + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("GetProductInfo(productId:" + productId + ") ... Failure: " + ex.Message);
                return new Response<Product>("Error in GetProductInfo: "+ ex.Message, 2);
            }
        }

        public Response<List<DataObject.OrderRecords.PastOrder>> GetPurchaseHistoryOfShop(int userId, int shopId)
        {
            try
            {
                Response < List < DataObject.OrderRecords.PastOrder >> res = facade.GetPurchaseHistoryOfShop(userId, shopId);
                if (res.HasError)
                {
                    Log.Error("GetPurchaseHistoryOfShop(userId:"+userId+", shopId:"+shopId+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetPurchaseHistoryOfShop(userId:" + userId + ", shopId:" + shopId + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("GetPurchaseHistoryOfShop(userId:" + userId + ", shopId:" + shopId + ") ... Failure: " + ex.Message);
                return new Response<List<DataObject.OrderRecords.PastOrder>>("Error in GetPurchaseHistoryOfShop: "+ ex.Message, 2);
            }
        }

        public Response<List<DataObject.OrderRecords.PastUserOrder>> GetPurchaseHistoryOfUser(int userId, string username)
        {
            try
            {
                Response < List < DataObject.OrderRecords.PastUserOrder >> res = facade.GetPurchaseHistoryOfUser(userId, username);
                if (res.HasError)
                {
                    Log.Error("GetPurchaseHistoryOfUser(userId:"+userId+", username:"+username+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetPurchaseHistoryOfUser(userId:" + userId + ", username:" + username + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("GetPurchaseHistoryOfUser(userId:" + userId + ", username:" + username + ") ... Failure: " + ex.Message);
                return new Response<List<DataObject.OrderRecords.PastUserOrder>>("Error in GetPurchaseHistoryOfUser: " + ex.Message, 2);
            }
        }

        public Response<Shop> GetShopInfo(int shopId)
        {
            try
            {
                Response < Shop > res = facade.GetShopInfo(shopId);
                if (res.HasError)
                {
                    Log.Error("GetShopInfo(shopId:"+shopId+") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetShopInfo(shopId:" + shopId + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("GetShopInfo(shopId:" + shopId + ") ... Failure: " + ex.Message);
                return new Response<Shop>("Error in GetShopInfo: " + ex.Message, 2);
            }
        }


        public Response<Shop> GetShopInfo(string shopName)
        {
            try
            {
                Response<Shop> res = facade.GetShopInfo(shopName);
                if (res.HasError)
                {
                    Log.Error("GetShopInfo(shopId:" + shopName + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetShopInfo(shopId:" + shopName + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("GetShopInfo(shopId:" + shopName + ") ... Failure: " + ex.Message);
                return new Response<Shop>("Error in GetShopInfo: " + ex.Message, 2);
            }
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="memberId">memder that did the action</param>
        /// <param name="shopId">shop id</param>
        /// <param name="type">type of predict: And, Or, Xor, Amount Less, Amount More, Item In Basket</param>
        /// <param name="firstParam">ID of first Predicate </param>
        /// <param name="secondParam">ID of secend Predicate</param>
        /// <returns></returns>
        public Response<int> BuildDiscountPredicate(int memberId, int shopId, string type, int firstParam, int? secondParam = 0)
        {
            try
            {
                Response<int> res = facade.BuildDiscountPredicate(memberId, shopId, type, firstParam, secondParam);
                if (res.HasError)
                {
                    Log.Error("BuildDiscountPredicate(memberId:" + memberId + ", shopId:" + shopId + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("BuildDiscountPredicate(memberId:" + memberId + ", shopId:" + shopId + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Success: ");

                return facade.BuildDiscountPredicate(memberId, shopId, type, firstParam, secondParam);
            }
            catch (Exception ex)
            {
                Log.Error("BuildDiscountPredicate(memberId:" + memberId + ", shopId:" + shopId + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in BuildDiscountPredicate: " + ex.Message, 2);
            }
        }
        /// <summary>
        /// add / max between discounts 
        /// </summary>
        /// <param name="memberId">memder that did the action</param>
        /// <param name="shopId">shop id</param>
        /// <param name="description">description: for notes</param>
        /// <param name="type">type: Adding, Max</param>
        /// <param name="policies">list of policies' id</param>
        /// <returns></returns>
        public Response<int> AddDiscountPolicyNumeric(int memberId, int shopId, string description, string type,
            List<int> policies)
        {
            try
            {
                Response<int> res = facade.AddDiscountPolicyNumeric(memberId, shopId, description, type, policies);
                if (res.HasError)
                {
                    Log.Error("AddDiscountPolicyNumeric(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", policies:" + policies + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Error("AddDiscountPolicyNumeric(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", policies:" + policies + ") ... Success: ");
                return res;

            }
            catch (Exception ex)
            {
                Log.Error("AddDiscountPolicyNumeric(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", policies:" + policies + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddDiscountPolicyNumeric: " + ex.Message, 2);
            }
        }
        /// <summary>
        /// if predicate work => then do policy
        /// </summary>
        /// <param name="memberId">memder that did the action</param>
        /// <param name="shopId">shop id</param>
        /// <param name="description">description: for notes</param>
        /// <param name="policyId">policy that created</param>
        /// <param name="predicateId">predicate that created in shop</param>
        /// <returns>policyId</returns>
        public Response<int> AddDiscountPolicyConditional(int memberId, int shopId, string description, int policyId,
            int predicateId)
        {
            try
            {
                Response<int> res = facade.AddDiscountPolicyConditional(memberId, shopId, description, policyId, predicateId);
                if (res.HasError)
                {
                    Log.Error("AddDiscountPolicyConditional(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", policyId:" + policyId + ", predicateId:" + predicateId + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddDiscountPolicyConditional(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", policyId:" + policyId + ", predicateId:" + predicateId + ") ... Success: ");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddDiscountPolicyConditional(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", policyId:" + policyId + ", predicateId:" + predicateId + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddDiscountPolicyConditional: " + ex.Message, 2);
            }
        }

        /// <summary>
        /// if predicta work => then do policy
        /// </summary>
        /// <param name="memberId">memder that did the action</param>
        /// <param name="shopId">shop id</param>
        /// <param name="description">description: for notes</param>
        /// <param name="type">type = And, Or, Xor</param>
        /// <param name="policyId">policy to do</param>
        /// <param name="predicateId">predicate to check</param>
        /// <returns>policyId</returns>
        public Response<int> AddDiscountPolicyLogical(int memberId, int shopId, string description, string type, int policyId, int predicateId)
        {
            try
            {
                Response<int> res = facade.AddDiscountPolicyLogical(memberId, shopId, description, type, policyId, predicateId);
                if (res.HasError)
                {
                    Log.Error("AddDiscountPolicyLogical(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", policyId:" + policyId + ", predicateId:" + predicateId + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddDiscountPolicyLogical(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", policyId:" + policyId + ", predicateId:" + predicateId + ") ... Success: ");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddDiscountPolicyLogical(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", policyId:" + policyId + ", predicateId:" + predicateId + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddDiscountPolicyLogical: " + ex.Message, 2);
            }
        }
        /// <summary>
        /// add discount policy - basic: regular discount
        /// </summary>
        /// <param name="memberId">memder that did the action</param>
        /// <param name="shopId">shop id</param>
        /// <param name="description">description: for notes</param>
        /// <param name="discount">amount of discount between 0 to 1</param>
        /// <param name="type">type: Shop, Item, Category</param>
        /// <param name="firstParam">param the is int</param>
        /// <param name="secondParam">param the is string</param>
        /// <returns></returns>
        public Response<int> AddDiscountPolicyBasic(int memberId, int shopId, string description, double discount, string type,
            int? firstParam = 0, string? secondParam = "")
        {
            try
            {
                Response<int> res = facade.AddDiscountPolicyBasic(memberId, shopId, description, discount, type, firstParam, secondParam);
                if (res.HasError)
                {
                    Log.Error("AddDiscountPolicyBasic(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", discount:" + discount + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddDiscountPolicyBasic(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", discount:" + discount + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Success: ");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddDiscountPolicyBasic(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", discount:" + discount + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddDiscountPolicyBasic: " + ex.Message, 2);
            }
            return facade.AddDiscountPolicyBasic(memberId, shopId, description, discount, type, firstParam, secondParam);
        }
        public Response<bool> ApplyDiscountPolicy(int memberId, int shopId, int policyid)
        {
            try
            {
                Response<bool> res = facade.ApplyDiscountPolicy(memberId, shopId, policyid);
                if (res.HasError)
                {
                    Log.Error("ApplyDiscountPolicy(memberId:" + memberId + ", shopId:" + shopId + ", policyid:" + policyid + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("ApplyDiscountPolicy(memberId:" + memberId + ", shopId:" + shopId + ", policyid:" + policyid + ") ... Success: ");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("ApplyDiscountPolicy(memberId:" + memberId + ", shopId:" + shopId + ", policyid:" + policyid + ") ... Failure: " + ex.Message);
                return new Response<bool>("Error in ApplyDiscountPolicy: " + ex.Message, 2);
            }
        }
        /// <summary>
        /// IF I can buy - by predicate
        /// </summary>
        /// <param name="memberId">memder that did the action</param>
        /// <param name="shopId">shop id</param>
        /// <param name="description">description: for notes</param>
        /// <param name="type">type: Lower Age, Date Sale, Before Hour, After Hour, Item Amount Less, Item Amount More, Item In Basket</param>
        /// <param name="appliesOn">appliesOn: Shop, Item, Category</param>
        /// <param name="firstParam"></param>
        /// <param name="secondParam"></param>
        /// <param name="thirdParam"></param>
        /// <param name="forthParam"></param>
        /// <returns>sale policy ID</returns>
        public Response<int> AddSalePolicyRestriction(int memberId, int shopId, string description,
            string? type = "", string? appliesOn = "Shop", int? firstParam = 0, int? secondParam = 0,
            int? thirdParam = 0, string? forthParam = "",int? fifthParam = 0)
        {
            try
            {
                Response<int> res = facade.AddSalePolicyRestriction(memberId, shopId, description, type, appliesOn, firstParam, secondParam, thirdParam, forthParam, fifthParam);
                if (res.HasError)
                {
                    Log.Error("AddSalePolicyRestriction(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", appliesOn:" + appliesOn + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ", thirdParam:" + thirdParam + ", forthParam:" + forthParam + ", fifthParam:" + fifthParam + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddSalePolicyRestriction(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", appliesOn:" + appliesOn + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ", thirdParam:" + thirdParam + ", forthParam:" + forthParam + ", fifthParam:" + fifthParam + ") ... Success: ");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddSalePolicyRestriction(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", appliesOn:" + appliesOn + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ", thirdParam:" + thirdParam + ", forthParam:" + forthParam + ", fifthParam:" + fifthParam + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddSalePolicyRestriction: " + ex.Message, 2);
            }
        }
        /// <summary>
        /// logical between sale policies
        /// </summary>
        /// <param name="memberId">memder that did the action</param>
        /// <param name="shopId">shop id</param>
        /// <param name="description">description: for notes</param>
        /// <param name="type">type: And, Or, Implies</param>
        /// <param name="firstParam">first policy</param>
        /// <param name="secondParam">second policy</param>
        /// <returns>sale policy ID</returns>
        public Response<int> AddSalePolicyLogical(int memberId, int shopId, string description, string? type = "",
            int? firstParam = 0, int? secondParam = 0)
        {
            try
            {
                Response<int> res = facade.AddSalePolicyLogical(memberId, shopId, description, type, firstParam, secondParam);
                if (res.HasError)
                {
                    Log.Error("AddSalePolicyLogical(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("AddSalePolicyLogical(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Success: ");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("AddSalePolicyLogical(memberId:" + memberId + ", shopId:" + shopId + ", description:" + description + ", type:" + type + ", firstParam:" + firstParam + ", secondParam:" + secondParam + ") ... Failure: " + ex.Message);
                return new Response<int>("Error in AddSalePolicyLogical: " + ex.Message, 2);
            }
        }

        public Response<bool> ApllySalePolicy(int memberId, int shopId, int policyid)
        {
            try
            {
                Response<bool> res = facade.ApllySalePolicy(memberId, shopId, policyid);
                if (res.HasError)
                {
                    Log.Error("ApllySalePolicy(memberId:" + memberId + ", shopId:" + shopId + ", policyid:" + policyid + ", type:" + res.ErrorMsg);
                    return res;
                }
                Log.Information("ApllySalePolicy(memberId:" + memberId + ", shopId:" + shopId + ", policyid:" + policyid + ", type:"  + ") ... Success: ");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("ApllySalePolicy(memberId:" + memberId + ", shopId:" + shopId + ", policyid:" + policyid  + ex.Message);
                return new Response<bool>("Error in ApllySalePolicy: " + ex.Message, 2);
            }
        }

        public Response<string> GetDiscountPolicies(int memberId, int shopId)
        {
            try
            {
                Response<string> res = facade.GetDiscountPolicies(memberId, shopId);
                if (res.HasError)
                {
                    Log.Error("GetDiscountPolicies(memberId:" + memberId + ", shopId:" + shopId + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetDiscountPolicies(memberId:" + memberId + ", shopId:" + shopId + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("GetDiscountPolicies(memberId:" + memberId + ", shopId:" + shopId + ") ... Failure: " + ex.Message);
                return new Response<string>("Error in GetDiscountPolicies: " + ex.Message, 2);
            }
        }

        public Response<string> GetSalePolicies(int memberId, int shopId)
        {
            try
            {
                Response<string> res = facade.GetSalePolicies(memberId, shopId);
                if (res.HasError)
                {
                    Log.Error("GetSalePolicies(memberId:" + memberId + ", shopId:" + shopId + ") ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetSalePolicies(memberId:" + memberId + ", shopId:" + shopId + ") ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("GetSalePolicies(memberId:" + memberId + ", shopId:" + shopId + ") ... Failure: " + ex.Message);
                return new Response<string>("Error in GetSalePolicies: " + ex.Message, 2);
            }
        }

        public Response<List<Shop>> GetAllOpenShops()
        {
            try
            {
                Response < List < Shop >> res = facade.GetAllOpenShops();
                if (res.HasError)
                {
                    Log.Error("GetAllOpenShops() ... Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information("GetAllOpenShops() ... Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error("cannot Get All Open Shops " + ex.Message);
                return new Response<List<Shop>>("GetAllOpenShops() ... Failure: " + ex.Message, 2);
            }
        }

        public Response<Shop> GetShopByName(string shopName)
        {
            string str = "GetShopByName(shopname:" + shopName + ") ... ";
            try
            {
                Response < Shop > res = facade.GetShopByName(shopName);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<Shop>("Error in GetShop: " + ex.Message, 2);
            }
        }

        public Response<OrdersHistory> GetShopPurchaseHistory(int userId, int shopId)
        {
            string str = "GetShopPurchaseHistory(userId:" + userId + ", shopId:" + shopId + ") ... ";
            try
            {
                Response < OrdersHistory > res = facade.GetShopPurchaseHistory(userId, shopId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res; 
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<OrdersHistory>("Error in GetShopPurchaseHistory: "+ ex.Message, 2);
            }
        }


        public Response<int> makePayment(string shop_bank, PaymentDetails customer_payment, double amount)
        {
            string str = "MakePayement(shop_bank:" + shop_bank + ", amount:" + amount + ") ... ";
            try
            {
                Response<int> res = facade.makePayment(shop_bank, customer_payment, amount);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>(ex.Message, 2);
            }
        }

        public Response<Order> MakePurchase(int userId, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            string str = "MakePurchase(userId:" + userId + ") ... ";
            try
            {
                Response < Order > res = facade.MakePurchase(userId, paymentDetails, deliveryDetails);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<Order>("Error in MakePurchase: " + ex.Message, 2);
            }
        }

        public Response<Shop> OpenShop(int userId, string shopName, string shopAddress, string bank)
        {
            string str = "OpenShop(userId:" + userId + ", shopName:" + shopName + ", shopAddress" + shopAddress + ") ... ";
            try
            {
                Response < Shop > res = facade.OpenShop(userId, shopName, shopAddress, bank);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<Shop>("Error in OpenShop: " + ex.Message, 2);
            }
        }

        public Response<int> OpenClosedShop(int userId, int shopId)
        {
            string str = "OpenClosedShop(userId:" + userId + ", shopId:" + shopId + ") ... ";
            try
            {
                Response<int> res = facade.OpenClosedShop(userId, shopId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>("Error in OpenShop: " + ex.Message, 2);
            }
        }
       


        public Response<int> RemoveProductFromCart(int userId,int shopId ,int productId, int amountToRemove)
        {
            string str = "RemoveProductFromCart(userId:" + userId + ", shopId:" + shopId + ", productId:" + productId + ", amountToRemove:" + amountToRemove + ") ... ";
            try
            {
                Response<int> res = facade.RemoveProductFromCart(userId, shopId, productId, amountToRemove);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>("Error in RemoveProductFromCart: " + ex.Message, 2);
            }
        }
        public Response<bool> EditProductInCart(int userId, int shopId, int productId, int amount)
        {
            string str = "EditProductInCart(userId:" + userId + ", shopId:" + shopId + ", productId:" + productId + ", amount:" + amount + ") ... ";
            try
            {
                Response<bool> res = facade.EditCart(userId, shopId, productId, amount);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>("Error in EditProductInCart: " + ex.Message, 2);
            }
        }

        public Response<bool> RemoveProductFromShop(int userId, int shopId, int productId)
        {
            string str = "RemoveProductFromShop(userId:" + userId + ", shopId:" + shopId + ", productId:" + productId + ") ... ";
            try
            {
                Response<bool> res = facade.RemoveProductFromShop(userId, shopId, productId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>("Error in RemoveProductFromShop: " + ex.Message, 2);
            }
        }

        public Response<List<Product>> SearchProducts(string? productName = null, int? productId = null, int? minPrice = null, int? maxPrice = null, string ? category = null)
        {
            string str = "SearchProduct() ... ";
            try
            {
                Response < List < Product >>res = facade.SearchProducts(productName, productId, minPrice, maxPrice, category);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<List<Product>>("Error in SearchProducts: " + ex.Message, 2);
            }
        }

        public Response<List<(Product,int)>> GetShopProducts(int shopId)
        {
            string str = "GetShopProducts(shopId:" + shopId + ") ... ";
            try
            {
                Response<List<(Product, int)>> res = facade.GetShopProducts(shopId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<List<(Product, int)>>("Error in GetShopProducts: " + ex.Message, 2);
            }
        }

        public Response<List<Product>> GetShopProductsWithoutQuantity(int shopId)
        {
            string str = "GetShopProductsWithoutQuantity(shopId:" + shopId + ") ... ";
            try
            {
                Response<List<Product>> res = facade.GetShopProductsWithoutQuantity(shopId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<List<Product>>("Error in GetShopProductsWithoutQuantity: " + ex.Message, 2);
            }
        }

        public Response<int> EditAmountInStock(int userId, int shopId, int productId, int amount)
        {
            string str = "EditAmountInStock(userId:" + userId + " shopId: " + shopId+ " productId: "+ productId+" amount: "+amount+") ... ";
            try
            {
                Response<int> res = facade.EditAmountToProductInStock(userId, shopId, productId, amount);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>("Error in EditAmountInStock: " + ex.Message, 2);
            }
        }
    }
}