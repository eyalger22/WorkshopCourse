using Market.DataObject;
using Market.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = Market.DataObject.OrderRecords.Order;
using Shop = Market.DataObject.Shop;
using ShoppingCart = Market.DataObject.ShoppingCart;

namespace TestMarket.Tests
{
    internal class TestDiscountPolicies
    {

        Proxy bridge;
        int user, user2;
        int shopId, shopId2;
        int p1, p2, p3;

        [SetUp]
        public void Setup()
        {
            bridge = new Proxy();
            bridge.r = new Real(Service.GetService());
            bridge.InitTheSystem();
            bridge.Register("eyal123", "eyal123", "eyal@gmail.com", "beer sheve");
            bridge.Register("dan123", "dan123", "dan@gmail.com", "beer sheve");
            bridge.Register("danial123", "danial123", "danial@gmail.com", "beer sheve");
            Response<int> u = bridge.EnterGuest();
            user = u.Value;
            Response<int> u2 = bridge.Login("dan123", "dan123");
            user2 = u2.Value;
            Response<Shop> s = bridge.OpenShop(user2, "shop1", "Tel Aviv", "0");
            shopId = s.Value.ShopId;

            Response<Shop> s2 = bridge.OpenShop(user2, "shop2", "Tel Aviv", "0");
            shopId2 = s2.Value.ShopId;
            Response<int> p = bridge.AddProductToShop(user2, shopId, "product1", 10, "category1", "");
            p1 = p.Value;
        }

        [TearDown]
        public void clean()
        {
        }

        [Test]
        public void TestDiscountsBasicMadeSuccess()
        {
            Response<int> res = bridge.AddDiscountPolicyBasic(user2, shopId, "", 0.5, "Item", p1);
            Assert.IsFalse(res.HasError, res.ErrorMsg);
            res = bridge.AddDiscountPolicyBasic(user2, shopId, "", 0.6, "Category", 0, "category1");
            Assert.IsFalse(res.HasError, res.ErrorMsg);

        }

        [Test]
        public void TestDiscountsBasicMadeFail()
        {
            Response<int> res = bridge.AddDiscountPolicyBasic(user2, shopId, "", 0.5, "Item", p1);
            Assert.IsFalse(res.HasError, res.ErrorMsg);
            res = bridge.AddDiscountPolicyBasic(user, shopId, "", 0.6, "Category", 0, "category1");
            Assert.IsTrue(res.HasError, "Should not be able to add discount policy to shop that is not yours");
        }

        [Test]
        public void TestDiscountsPredicateMadeSuccess()
        {
            Response<int> res = bridge.BuildDiscountPredicate(user2, shopId, "Amount Less", 2);
            Assert.IsFalse(res.HasError, res.ErrorMsg);
            var res1 = bridge.BuildDiscountPredicate(user2, shopId, "Item In Basket", p1);
            Assert.IsFalse(res1.HasError, res1.ErrorMsg);
            var res2 = bridge.BuildDiscountPredicate(user2, shopId, "And", res.Value, res1.Value);
            Assert.IsFalse(res2.HasError, res2.ErrorMsg);
            var res3 = bridge.BuildDiscountPredicate(user2, shopId, "Or", res2.Value, res1.Value);
            Assert.IsFalse(res3.HasError, res3.ErrorMsg);
        }

        [Test]
        public void TestDiscountsPredicateMadeFail()
        {
            Response<int> resB = bridge.AddDiscountPolicyBasic(user2, shopId, "", 0.5, "Item", p1);
            Assert.IsFalse(resB.HasError, resB.ErrorMsg);
            resB = bridge.AddDiscountPolicyBasic(user2, shopId, "", 0.6, "Item", p1);
            Assert.IsFalse(resB.HasError, resB.ErrorMsg);
            resB = bridge.AddDiscountPolicyBasic(user2, shopId, "", 0.7, "Item", p1);
            Assert.IsFalse(resB.HasError, resB.ErrorMsg);
            Response<int> res = bridge.BuildDiscountPredicate(user2, shopId, "Amount Less", 2);
            Assert.IsFalse(res.HasError, res.ErrorMsg);
            var res1 = bridge.BuildDiscountPredicate(user2, shopId, "Item In Basket", p1);
            Assert.IsFalse(res1.HasError, res1.ErrorMsg);
            var res2 = bridge.BuildDiscountPredicate(user2, shopId, "And", res.Value + 5, res1.Value + 5);
            Assert.IsTrue(res2.HasError, "Should not be able to build predicate with invalid ids");
            var res3 = bridge.BuildDiscountPredicate(user2, shopId, "And", resB.Value, res1.Value);
            Assert.IsTrue(res3.HasError, "Should not be able to build predicate with a not predicate discount");
        }

        

        
    }
}
