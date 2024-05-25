using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DataObject;
using Market.ServiceLayer;

namespace TestMarket.Tests
{
    internal class TestDataBase
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
            Response<int> p = bridge.AddProductToShop(user, shopId, "product1", 10, "category1", "");
            p1 = p.Value;
            Response<int> res1 = bridge.AddAmountToProductInStock(user, shopId, p1, 10);
        }
        [TearDown]
        public void clean()
        {

        }

        // tests for data integrity

        [Test]
        public void TestMemberSavingAfterReset()
        {
            bridge.Register("user1", "pass1", "u1@gmail.com", "Beer Sheva");
            bridge.InitTheSystem();
            Response<int> res1 = bridge.Login("user1", "pass1");
            Assert.IsFalse(res1.HasError);
        }

        [Test]
        public void TestShopSavingAfterReset()
        {
            bridge.InitTheSystem();
            //Response<int> res1 = bridge.Login("dan123", "dan123");
            //Assert.IsFalse(res1.HasError);
            Response<Shop> res2 = bridge.GetShopInfo(shopId);
            Assert.IsFalse(res2.HasError);
            Shop s1 = res2.Value;
            Assert.That("shop1", Is.EqualTo(s1.Name));
            Assert.That("Tel Aviv", Is.EqualTo(s1.ShopAddress));
        }

        [Test]
        public void TestShopProductSavingAfterReset()
        {
            bridge.InitTheSystem();
            Response<int> res1 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res1.HasError);
            Response<Product> res2 = bridge.GetProductInfo(p1);
            Assert.IsFalse(res2.HasError);
            Assert.That("product1", Is.EqualTo(res2.Value.Name));
            Assert.That(10, Is.EqualTo(res2.Value.Price));
            Assert.That("category1", Is.EqualTo(res2.Value.Category));
            Assert.That("", Is.EqualTo(res2.Value.Description));
            //Response<Shop> res3 = bridge.GetShopInfo(shopId);
            //Assert.IsFalse(res3.HasError);
            //Shop s1 = res3.Value;
            //Assert.That("", Is.EqualTo(s1.))
        }

        [Test]
        public void TestShopEmployeeSavingAfterReset()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "danial123");
            Assert.IsFalse(res1.HasError);
            bridge.InitTheSystem();
            Response<int> res2 = bridge.Login("eyal123", "eyal123");
            Assert.IsFalse(res2.HasError);
            Response<User> res3 = bridge.GetEmployeeInformation(res2.Value, shopId, "danial123");
            Assert.IsFalse(res3.HasError);
            User u1 = res3.Value;
            Assert.That("danial123", Is.EqualTo(u1.Name));
        }

        [Test]
        public void TestShopManagerPermissionsSavingAfterReset()
        {
            Response<bool> res1 = bridge.AddManager(user, shopId, "danial123");
            Assert.IsFalse(res1.HasError);
            Response<string> res4 = bridge.AddPermission(user, shopId, "danial123", PermissionsEnum.Permission.MANAGE_POLICIES);
            Assert.IsFalse(res4.HasError);
            bridge.InitTheSystem();
            Response<int> res2 = bridge.Login("eyal123", "eyal123");
            Assert.IsFalse(res2.HasError);
            Response<User> res3 = bridge.GetEmployeeInformation(res2.Value, shopId, "danial123");
            Assert.IsFalse(res3.HasError);
            User u1 = res3.Value;
            Assert.That("danial123", Is.EqualTo(u1.Name));
            Assert.IsTrue(u1.Permissions.Contains(PermissionsEnum.Permission.MANAGE_ITEMS));
            Assert.IsTrue(u1.Permissions.Contains(PermissionsEnum.Permission.MANAGE_DISCOUNTS));
            Assert.IsTrue(u1.Permissions.Contains(PermissionsEnum.Permission.GET_HISTORY_ORDERS));
            Assert.IsTrue(u1.Permissions.Contains(PermissionsEnum.Permission.RESPONSE_TO_USERS));
            Assert.IsTrue(u1.Permissions.Contains(PermissionsEnum.Permission.SHOP_MANAGER));
            Assert.IsTrue(u1.Permissions.Contains(PermissionsEnum.Permission.MANAGE_POLICIES));
        }

        [Test]
        public void TestMemberShoppinCartSavingAfterReset()
        {
            Response<bool> res1 = bridge.AddProductToCart(user2, shopId, p1, 1);
            Assert.IsFalse(res1.HasError);
            bridge.InitTheSystem();
            Response<int> res2 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res2.HasError);
            Response<ShoppingCart> res3 = bridge.ViewShoppingCart(res2.Value);
            Assert.IsFalse(res3.HasError);
            List<(string, ShoppingBasket)> baskets = res3.Value.Baskets;
            Assert.That(1, Is.EqualTo(baskets.Count));
            Assert.That("shop1", Is.EqualTo(baskets.First().Item1));
            ShoppingBasket basket1 = baskets.First().Item2;
            Assert.That(1, Is.EqualTo(basket1.Products.Count));
            Assert.That("product1", Is.EqualTo(basket1.Products.First().Item1.Name));
            Assert.That(1, Is.EqualTo(basket1.Products.First().Item2));
        }

        // data base connection error tests

    }
}
