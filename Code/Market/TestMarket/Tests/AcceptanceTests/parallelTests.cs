using Market.DataObject;
using Market.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Order = Market.DataObject.OrderRecords.Order;
using Shop = Market.DataObject.Shop;
using Market.DomainLayer.Market;

namespace TestMarket.Tests
{
    internal class parallelTests
    {
        Proxy bridge;
        int user, user2;
        int shopId, shopId2;
        int p1;


        [SetUp]
        public void SettUp()
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
            Item.initItemId();
        }

        [TearDown]
        public void clean()
        {
            bridge.InitTheSystem();
        }

        [Test]
        public void DobleNameRegister()
        {
            bool res1 = false;
            bool res2 = false;
            Thread t1 = new Thread(() =>
            {
                Response<bool> r1 = bridge.Register("user", "pass", "email@gmail.com", "addr");
                res1 = !r1.HasError;
            });
            Thread t2 = new Thread(() =>
            {
                Response<bool> r2 = bridge.Register("user", "pass", "email@gmail.com", "addr");
                res2 = !r2.HasError;
            });
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            if((res1 && res2)||!(res1 || res2) ){
                Assert.Fail();
            }
        }
        [Test]
        public void TestDobleRegisterSuccess()
        {
            bool res1 = false;
            bool res2 = false;
            Thread t1 = new Thread(() =>
            {
                Response<bool> r1 = bridge.Register("user", "pass", "email@gmail.com", "addr");
                res1 = !r1.HasError;
            });
            Thread t2 = new Thread(() =>
            {
                Response<bool> r2 = bridge.Register("user1", "pass1", "email1@gmail.com", "addr1");
                res2 = !r2.HasError;
            });
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            if ((res1 && res2))
            {
                return;
            }
            Assert.Fail();
        }


        [Test]
        public void TestDoublePromotionOwner()
        {
            bridge.AddOwner(user2, shopId, "eyal123");
            Response<int> r = bridge.Login("eyal123", "eyal123");
            int newId = r.Value;
            bool res1 = false;
            bool res2 = false;
            Thread t1 = new Thread(() =>
            {
                Response<bool> r1 = bridge.AddOwner(newId, shopId, "danial123");
                res1 = !r1.HasError;
            });
            Thread t2 = new Thread(() =>
            {
                Response<bool> r2 = bridge.AddOwner(user2, shopId, "danial123");
                res2 = !r2.HasError;
            });
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            if ((res1 && res2) || !(res1 || res2))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TestOpenShops()
        {
            bool res1 = false;
            bool res2 = false;
            Thread t1 = new Thread(() =>
            {
                Response<Shop> r1 = bridge.OpenShop(user2,"newShop1","address1", "0");
                res1 = !r1.HasError;
            });
            Thread t2 = new Thread(() =>
            {
                Response<Shop> r2 = bridge.OpenShop(user2, "newShop1", "address2", "0");
                res2 = !r2.HasError;
            });
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            if ((res1 && res2) || !(res1 || res2))
            {
                Assert.Fail();
            }
        }
        [Test]
        public void DoublePurchaseFail()
        {
            bridge.AddAmountToProductInStock(user2, shopId, p1, 2);
            Response<int> r = bridge.Login("eyal123", "eyal123");
            int user3 = r.Value;
            PaymentDetails paymentDetails = new PaymentDetails("", "", "", default, "", "");
            DeliveryDetails deliveryDetails = new DeliveryDetails("home", "c3", "USA", "LA", "123456");
            bridge.AddProductToCart(user2, shopId, p1, 2);
            bridge.AddProductToCart(user3, shopId, p1, 2);
            bool res1 = false;
            bool res2 = false;
            Thread t1 = new Thread(() =>
            {
                Response<Order> r1 = bridge.MakePurchase(user2, paymentDetails, deliveryDetails);
                res1 = !r1.HasError;
            });
            Thread t2 = new Thread(() =>
            {
                Response<Order> r2 = bridge.MakePurchase(user3, paymentDetails, deliveryDetails);
                res2 = !r2.HasError;
            });
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            if ((res1 && res2) || !(res1 || res2))
            {
                Assert.Fail();
            }

        }


    }
}
