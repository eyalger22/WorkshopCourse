using Market.DataObject;
using Market.DomainLayer.Market;
using Market.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop = Market.DataObject.Shop;
using Order = Market.DataObject.OrderRecords.Order;
using Market.DataObject.OrderRecords;

namespace TestMarket.Tests
{
    internal class TestShopManager
    {


        Proxy bridge;
        int user, user2;
        int shopId, shopId2;
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
            bridge.AddManager(user, shopId, "dan123");
            //Response<int> s2 = bridge.OpenShop(user, "shop2", "Tel Aviv");
            //shopId2 = s.Value;

        }
        [TearDown]
        public void clean()
        {
            bridge.Logout(user);
            bridge.CloseShop(user, shopId);
            bridge.Logout(user2);
        }


        [Test]
        public void successActionMangerAddProduct()
        {

            Response<int> res = bridge.AddProductToShop(user2, shopId, "name", 2, "category", "description");
            Item.initItemId();
            Assert.IsFalse(res.HasError);
            Response<Product> res1 = bridge.GetProductInfo(res.Value);
            Assert.IsFalse (res1.HasError);
            Assert.AreEqual("name", res1.Value.Name);
        }
        [Test]
        public void successActionMangerSearchProduct()
        {
            Response<OrdersHistory> res = bridge.GetShopPurchaseHistory(user2, shopId);
            Assert.IsFalse(res.HasError);
        }

        [Test]
        public void failActionManeger1()
        {
            Response<bool> res = bridge.AddManager(user2, shopId, "danial123");
            Assert.IsTrue(res.HasError);
            //TODO: check that a manager wasnt added to the shop
            Response<User> res2 = bridge.GetEmployeeInformation(user, shopId, "danial123");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void failActionManeger2()
        {
            Response<int> res = bridge.CloseShop(user2, shopId);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void addResponsibilityMangerSuccess()
        {
            Response<string> r1 = bridge.AddPermission(user, shopId, "dan123", PermissionsEnum.Permission.ADD_MANAGER);
            Assert.IsFalse(r1.HasError);
            Response<bool> res = bridge.AddManager(user2, shopId, "danial123");
            Assert.IsFalse(res.HasError);
            //TODO: check that the manager was added to the shop
            Response<User> res2 = bridge.GetEmployeeInformation(user, shopId, "danial123");
            //Assert.That(0, Is.EqualTo(res.ErrorKind));
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value.Permissions.Contains(PermissionsEnum.Permission.MANAGE_ITEMS));
            Assert.IsTrue(res2.Value.Permissions.Contains(PermissionsEnum.Permission.MANAGE_DISCOUNTS));
            Assert.IsTrue(res2.Value.Permissions.Contains(PermissionsEnum.Permission.GET_HISTORY_ORDERS));
            Assert.IsTrue(res2.Value.Permissions.Contains(PermissionsEnum.Permission.RESPONSE_TO_USERS));
            Assert.IsTrue(res2.Value.Permissions.Contains(PermissionsEnum.Permission.SHOP_MANAGER));
        }


        [Test]
        public void removePermission()
        {
            Response<bool> r1 = bridge.RemovePermission(user, shopId,"dan123",PermissionsEnum.Permission.MANAGE_ITEMS);
            Assert.IsFalse(r1.HasError);
            Response<int> res = bridge.AddProductToShop(user2, shopId, "name", 2, "category", "description");
            Item.initItemId();
            Assert.IsTrue(res.HasError);
        }
    }
}
