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
    internal class TestUserBuying
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
            Response<int> u = bridge.Login("eyal123", "eyal123");
            user = u.Value;
            Response<Shop> s = bridge.OpenShop(user, "shop1", "Tel Aviv", "0");
            shopId = s.Value.ShopId;
            Response<int> u2 = bridge.Login("dan123", "dan123");
            user2 = u2.Value;
            Response<Shop> s2 = bridge.OpenShop(user, "shop2", "Tel Aviv", "0");
            shopId2 = s2.Value.ShopId;
            Response<int> p = bridge.AddProductToShop(user, shopId, "product1", 10 , "category1", "");
            p1 = p.Value;
        }

        [TearDown]
        public void clean()
        {
        }

        //tests for get shop information

        [Test]
        public void TestGetShopInfoSuccess()
        {
            Response<Shop> res = bridge.GetShopInfo(shopId);
            Assert.IsFalse(res.HasError);
            Assert.That("shop1", Is.EqualTo(res.Value.Name));
            Assert.That("Tel Aviv", Is.EqualTo(res.Value.ShopAddress));
        }

        [Test]
        public void TestGetShopInfoFail()
        {
            Response<Shop> res = bridge.GetShopInfo(-1);
            Assert.IsTrue(res.HasError);
        }

        //tests for search products
        [Test]
        public void TestSearchProductsSuccess()
        {
            Response<List<Product>> res = bridge.SearchProducts("product1");
            Assert.IsFalse(res.HasError);
            Assert.That(1 , Is.EqualTo(res.Value.Count));
            Assert.That("product1", Is.EqualTo(res.Value[0].Name));
            Assert.That("category1", Is.EqualTo(res.Value[0].Category));
        }

        [Test]
        public void TestSearchProductsNotFind()
        {
            Response<List<Product>> res = bridge.SearchProducts("not product");
            Assert.IsFalse(res.HasError);
            Assert.That(0, Is.EqualTo(res.Value.Count));

        }

        //tests for save items to user's shopping basket
        [Test]
        public void TestSaveItemsToUserShoppingBasketSuccess()
        {
            Response<bool> res = bridge.AddProductToCart(user, shopId, p1, 3);
            Assert.IsFalse(res.HasError);
            Assert.That(true, Is.EqualTo(res.Value));
        }

        [Test]
        public void TestSaveItemsToUserShoppingBasketFailAmount0()
        {
            Response<bool> res = bridge.AddProductToCart(user, shopId, p1, 0);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestSaveItemsToUserShoppingBasketFailAmountNegative()
        {
            Response<bool> res = bridge.AddProductToCart(user, shopId, p1, -3);
            Assert.IsTrue(res.HasError);
        }

        //tests for Checking shopping cart contents

        [Test]
        public void TestCheckingShoppingCartContentsSuccessEmptyCart()
        {
            Response<ShoppingCart> res2 = bridge.ViewShoppingCart(user);
            Assert.IsFalse(res2.HasError);
        }

        [Test]
        public void TestCheckingShoppingCartContentsSuccess()
        {
            Response<bool> res = bridge.AddProductToCart(user, shopId, p1, 3);
            Assert.IsFalse(res.HasError);
            Assert.That(true, Is.EqualTo(res.Value));
            Response<ShoppingCart> res2 = bridge.ViewShoppingCart(user);
            Assert.IsFalse(res2.HasError);
        }

        [Test]
        public void TestCheckingShoppingCartContentsFail()
        {
            Response<ShoppingCart> res2 = bridge.ViewShoppingCart(-2);
            Assert.IsTrue(res2.HasError);
        }

        //tests for purchase shopping cart

        [Test]
        public void TestPurchaseShoppingCartSuccess()
        {
            Response<bool> res = bridge.AddProductToCart(user, shopId, p1, 3);
            Assert.IsFalse(res.HasError);
            Assert.That(true, Is.EqualTo(res.Value));
            Response<int> res1 = bridge.AddAmountToProductInStock(user, shopId, p1, 3);
            Assert.IsFalse(res1.HasError, res1.ErrorMsg);
            PaymentDetails paymentDetails = new PaymentDetails("123456", "7", "2026", "Eyal123", "123", "123456789");
            DeliveryDetails deliveryDetails = new DeliveryDetails("home", "c3", "USA", "LA", "123456");
            Response<Order> res2 = bridge.MakePurchase(user, paymentDetails, deliveryDetails);

            Assert.IsFalse(res2.HasError);
        }

        [Test]
        public void TestPurchaseShoppingCartFailEmptyCart()
        {
            PaymentDetails paymentDetails = new PaymentDetails("123456", "7", "2026", "Eyal123", "123", "123456789");
            DeliveryDetails deliveryDetails = new DeliveryDetails("home", "c3", "USA", "LA", "123456");
            Response<Order> res2 = bridge.MakePurchase(user, paymentDetails, deliveryDetails);
            Assert.IsTrue(res2.HasError);
        }

    }
}
