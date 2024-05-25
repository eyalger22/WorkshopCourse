using Market.DataObject;
using Market.DataObject.OrderRecords;
using Market.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarket.Tests
{
    internal class TestSystemManager
    {

        Proxy bridge;
        int admin;
        int user, user2;
        int shopId, shopId2;
        int p1;

        public void Setup()
        {
            bridge = new Proxy();
            bridge.r = new Real(Service.GetService());
            Response<int> res = bridge.Login("admin", "admin");
            admin = res.Value;
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
            //Response<int> s2 = bridge.OpenShop(user, "shop2", "Tel Aviv");
            //shopId2 = s.Value;
        }

        [Test]
        public void UnregisterSuccsess() {
            Response<bool> res = bridge.Unregister(admin, "dan123");
            Assert.IsFalse(res.Value);
            //TODO: check that the user was removed
            Response<int> res1 = bridge.Login("dan123", "dan123");
            Assert.IsTrue(res1.HasError);
        }

        public void UnregisterFail()
        {
            Response<bool> res = bridge.Unregister(admin, "eyal123");
            Assert.IsTrue(res.Value);
        }
        [Test]
        public void UetHistoryOfShopSuccsess()
        {
            Response<List<PastOrder>> res = bridge.GetPurchaseHistoryOfShop(admin, shopId);
            Assert.IsFalse(res.HasError);

        }
        [Test]
        public void GetHistoryOfShopFail()
        {
            Response<List<PastOrder>> res = bridge.GetPurchaseHistoryOfShop(user, shopId);
            Assert.IsTrue(res.HasError);

        }

        [Test]
        public void GetUsersInfoSuccsess()
        {
            Response<List<PastUserOrder>> res = bridge.GetPurchaseHistoryOfUser(admin, "eyal123");
            Assert.IsFalse(res.HasError);
        }

        /*[Test]
        public void getUsersInfoFailOnAdminRequest()
        {
            Response<List<PastUserOrder>> res = bridge.GetPurchaseHistoryOfUser(admin, "admin");
            Assert.IsTrue(res.HasError);
        }*/


        [Test]
        public void GetUsersInfoFailNotAdmin()
        {
            Response<List<PastUserOrder>> res = bridge.GetPurchaseHistoryOfUser(user, "eyal123");
            Assert.IsTrue(res.HasError);
        }


    }
}
