using Market.DataObject;
using Market.ServiceLayer;
//using Market.DomainLayer.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarket.Tests
{
    internal class TestMember
    {
        Proxy bridge;
        int user;
        [SetUp]
        public void Setup()
        {
            bridge = new Proxy();
            bridge.r = new Real(Service.GetService());
            bridge.InitTheSystem();
            bridge.Register("eyal123", "eyal123", "eyal@gmail.com", "beer sheve");
            Response<int> v = bridge.Login("eyal123", "eyal123");
            user = v.Value;
        }

        [TearDown]
        public void clean()
        {
            bridge.Logout(user);
        }


        // tests for logout
        [Test]
        public void TestLogoutOfLoginUser()
        {
            Response<bool> res = bridge.Logout(user);
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
        }

        [Test]
        public void TestLogoutOfGuest()
        {
            int guest = bridge.EnterGuest().Value;
            Response<bool> res = bridge.Logout(guest);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestLogoutUser2Times()
        {
            Response<bool> res = bridge.Logout(user);
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
            Response<bool> res2 = bridge.Logout(user);
            Assert.IsTrue(res2.HasError);
        }

        //Tests for open shop
        [Test]
        [TestCase("shop1", "Natanyia")]
        [TestCase("shop2", "Natanyia")]
        public void TestOpenShopSuccess(string shopName, string adress)
        {
            Response<Shop> res = bridge.OpenShop(user, shopName, adress, "0");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value.Name == shopName);
            //Assert.IsTrue(res.Value.Address == adress);
            //TODO: check that the shop is in the system
            Response<Shop> res2 = bridge.GetShopInfo(res.Value.ShopId);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value.Name == shopName);
            Assert.IsTrue(res2.Value.ShopAddress == adress);
        }

        [Test]
        [TestCase("shop1", "Natanyia")]
        [TestCase("shop2", "Natanyia")]
        public void TestOpenShopFailShopExist(string shopName, string adress)
        {
            Response<Shop> res = bridge.OpenShop(user, shopName, adress, "0");
            Assert.IsFalse(res.HasError);
            Response<Shop> res2 = bridge.OpenShop(user, shopName, adress, "0");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        [TestCase("", "Natanyia")]
        [TestCase("Shop1", null)]
        [TestCase("Shop1", "")]
        [TestCase("", "")]
        [TestCase(null, "Natanyia")]
        public void TestOpenShopFailInvalidParameters(string shopName, string adress)
        {
            Response<Shop> res = bridge.OpenShop(user, shopName, adress, "0");
            Assert.IsTrue(res.HasError);
            //TODO: check that the shop wasnt added to the system
        }


        [Test]
        public void testSavingCartMember()
        {
            Response<Shop> res1 = bridge.OpenShop(user, "shop1", "Natanyia", "0");
            Assert.IsFalse(res1.HasError);
            Shop s = res1.Value;
            Response<int> res2 = bridge.AddProductToShop(user, s.ShopId, "milk", 5, "test", "test item");
            Assert.IsFalse(res2.HasError);
            int productid = res2.Value;
            Response<int> res3 = bridge.AddAmountToProductInStock(user, s.ShopId, productid, 5);
            Assert.IsFalse(res3.HasError);
            Response<bool> res4 = bridge.AddProductToCart(user, s.ShopId, productid, 1);
            Assert.IsFalse(res4.HasError);
            Response<bool> r = bridge.Logout(user);
            Assert.IsFalse(r.HasError);
            Response<int> res7 = bridge.Login("eyal123", "eyal123");
            Assert.IsFalse(res7.HasError);
            Response<int> res8 = bridge.RemoveProductFromCart(res7.Value, s.ShopId, productid, 1);
            Assert.IsFalse(res8.HasError);
        }

        [Test]
        public void testSavingCartGuest()
        {
            Response<Shop> res1 = bridge.OpenShop(user, "shop1", "Natanyia", "0");
            Assert.IsFalse(res1.HasError);
            Shop s = res1.Value;
            Response<int> res2 = bridge.AddProductToShop(user, s.ShopId, "milk", 5, "test", "test item");
            Assert.IsFalse(res2.HasError);
            int productid = res2.Value;
            Response<int> res3 = bridge.AddAmountToProductInStock(user, s.ShopId, productid, 5);
            Assert.IsFalse(res3.HasError);
            Response<int> res4 = bridge.EnterGuest();
            Assert.IsFalse(res4.HasError);
            int guestId = res4.Value;
            Response<bool> res5 = bridge.AddProductToCart(guestId, s.ShopId, productid, 1);
            Assert.IsFalse(res5.HasError);
            Response<int> res6 = bridge.Exit(guestId);
            Assert.IsFalse(res6.HasError);
            Response<int> res7 = bridge.EnterGuest();
            Assert.IsFalse(res7.HasError);
            guestId = res7.Value;
            Response<int> res8 = bridge.RemoveProductFromCart(guestId, s.ShopId, productid, 1);
            Assert.IsTrue(res8.HasError);


        }



    }
}
