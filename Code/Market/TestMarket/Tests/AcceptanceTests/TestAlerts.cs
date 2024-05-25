using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DataObject;
using Market.DataObject.OrderRecords;
using Market.ServiceLayer;


namespace TestMarket.Tests
{
    internal class TestAlerts
    {
        Proxy bridge;
        int user, user2, user3, newuser, newuser2;
        int shopId, shopId2;
        [SetUp]
        public void Setup()
        {
            bridge = new Proxy();
            bridge.r = new Real(Service.GetService());
            bridge.InitTheSystem();
            bridge.Register("eyal123", "eyal123", "eyal@gmail.com", "beer sheve");
            bridge.Register("dan123", "dan123", "dan@gmail.com", "beer sheve");
            bridge.Register("daniel123", "daniel123", "daniel@gmail.com", "beer sheve");
            bridge.Register("newuser123", "newuser123", "newuser123@gmail.com", "beer sheva");
            bridge.Register("newuser2123", "newuser2123", "newuser2123@gmail.com", "beer sheva");
            Response<int> u = bridge.Login("eyal123", "eyal123");
            user = u.Value;
            Response<Shop> s = bridge.OpenShop(user, "shop1", "Tel Aviv", "bank");
            shopId = s.Value.ShopId;
            Response<int> u2 = bridge.Login("dan123", "dan123");
            user2 = u2.Value;
            Response<int> res10 = bridge.Login("daniel123", "daniel123");
            user3 = res10.Value;
            Response<int> res11 = bridge.Login("newuser123", "newuser123");
            newuser = res11.Value;
            Response<int> res12 = bridge.Login("newuser2123", "newuser2123");
            newuser2 = res12.Value;
            //Response<int> s2 = bridge.OpenShop(user, "shop2", "Tel Aviv");
            //shopId2 = s.Value;


        }

        [TearDown]
        public void clean()
        {
            //bridge.Logout(user);
            bridge.CloseShop(user, shopId);
            //bridge.Logout(user2);
            //bridge.Logout(user3);
            //bridge.Logout(newuser);
            //bridge.Logout(newuser2);
        }

        [Test]
        public void TestSimpleHiredEmployeeDelayedAlertSuccess()
        {
            Response<bool> res0 = bridge.Logout(user2);
            Assert.IsFalse(res0.HasError);
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<int> res3 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res3.HasError);
            int u2 = res3.Value;
            Response<List<string>> res4 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res4.HasError);
            Assert.That(1, Is.EqualTo(res4.Value.Count));
        }
        [Test]
        public void TestSimpleHiredEmployeeDelayedAlertFailure()
        {
            Response<bool> res0 = bridge.Logout(user2);
            Assert.IsFalse(res0.HasError);
            Response<bool> res1 = bridge.AddOwner(newuser, shopId, "dan123");
            Assert.IsTrue(res1.HasError);
            Response<int> res3 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res3.HasError);
            int u2 = res3.Value;
            Response<List<string>> res4 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res4.HasError);
            Assert.That(0, Is.EqualTo(res4.Value.Count));
        }
        [Test]
        public void TestSimpleFiredEmployeeDelayedAlertSuccess()
        {
            Response<bool> res0 = bridge.Logout(user2);
            Assert.IsFalse(res0.HasError);
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res2.HasError);
            Response<int> res3 = bridge.Login("dan123","dan123");
            Assert.IsFalse(res3.HasError);
            int u2 = res3.Value;
            Response<List<string>> res4 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res4.HasError);
            Assert.That(2, Is.EqualTo(res4.Value.Count));
        }
        [Test]
        public void TestSimpleFiredEmployeeDelayedAlertFailure()
        {
            Response<bool> res0 = bridge.Logout(user2);
            Assert.IsFalse(res0.HasError);
            Response<bool> res1 = bridge.AddOwner(newuser, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<int> res3 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res3.HasError);
            int u2 = res3.Value;
            Response<List<string>> res4 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res4.HasError);
            Assert.That(0, Is.EqualTo(res4.Value.Count));
        }
        [Test]
        public void TestComplexFiredEmployeeDelayedAlertSuccess()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res21 = bridge.AddOwner(user, shopId, "daniel123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res22 = bridge.AddManager(user2, shopId, "newuser123");
            Assert.IsFalse(res22.HasError);
            Response<bool> res3 = bridge.Logout(user2);
            Assert.IsFalse(res3.HasError);
            Response<bool> res4 = bridge.Logout(user3);
            Assert.IsFalse(res4.HasError);
            Response<bool> res5 = bridge.Logout(newuser);
            Assert.IsFalse(res5.HasError);
            Response<bool> res6 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res6.HasError);
            Response<int> res7 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res7.HasError);
            int u2 = res7.Value;
            Response<List<string>> res8 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res4.HasError);
            Assert.That(2, Is.EqualTo(res8.Value.Count));
            Response<int> res9 = bridge.Login("daniel123", "daniel123");
            Assert.IsFalse(res9.HasError);
            int u3 = res9.Value;
            Response<List<string>> res10 = bridge.ReadAlerts(u3);
            Assert.IsFalse(res10.HasError);
            Assert.That(2, Is.EqualTo(res10.Value.Count));
            Response<int> res11 = bridge.Login("newuser123", "newuser123");
            Assert.IsFalse(res11.HasError);
            int nu = res11.Value;
            Response<List<string>> res12 = bridge.ReadAlerts(nu);
            Assert.IsFalse(res12.HasError);
            Assert.That(2, Is.EqualTo(res12.Value.Count));
        }
        [Test]
        public void TestComplexFiredEmployeeDelayedAlertFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res21 = bridge.AddOwner(user, shopId, "daniel123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res22 = bridge.AddManager(user2, shopId, "newuser123");
            Assert.IsFalse(res22.HasError);
            Response<bool> res3 = bridge.Logout(user2);
            Assert.IsFalse(res3.HasError);
            Response<bool> res4 = bridge.Logout(user3);
            Assert.IsFalse(res4.HasError);
            Response<bool> res5 = bridge.Logout(newuser);
            Assert.IsFalse(res5.HasError);
            Response<bool> res6 = bridge.RemoveOwner(newuser2, shopId, "dan123");
            Assert.IsTrue(res6.HasError);
            Response<int> res7 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res7.HasError);
            int u2 = res7.Value;
            Response<List<string>> res8 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res4.HasError);
            Assert.That(1, Is.EqualTo(res8.Value.Count));
            Response<int> res9 = bridge.Login("daniel123", "daniel123");
            Assert.IsFalse(res9.HasError);
            int u3 = res9.Value;
            Response<List<string>> res10 = bridge.ReadAlerts(u3);
            Assert.IsFalse(res10.HasError);
            Assert.That(1, Is.EqualTo(res10.Value.Count));
            Response<int> res11 = bridge.Login("newuser123", "newuser123");
            Assert.IsFalse(res11.HasError);
            int nu = res11.Value;
            Response<List<string>> res12 = bridge.ReadAlerts(nu);
            Assert.IsFalse(res12.HasError);
            Assert.That(1, Is.EqualTo(res12.Value.Count));
        }
        [Test]
        public void TestSimpleCloseShopDelayedAlertSuccess()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res3 = bridge.Logout(user2);
            Assert.IsFalse(res3.HasError);
            Response<int> res6 = bridge.CloseShop(user, shopId);
            Assert.IsFalse(res6.HasError);
            Response<int> res7 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res7.HasError);
            int u2 = res7.Value;
            Response<List<string>> res8 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res8.HasError);
            Assert.That(2, Is.EqualTo(res8.Value.Count));
            Response<List<string>> res13 = bridge.ReadAlerts(user);
            Assert.IsFalse(res13.HasError);
            Assert.That(1, Is.EqualTo(res13.Value.Count));
        }
        [Test]
        public void TestSimpleCloseShopDelayedAlertFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res3 = bridge.Logout(user2);
            Assert.IsFalse(res3.HasError);
            Response<int> res6 = bridge.CloseShop(newuser, shopId);
            Assert.IsFalse(res6.HasError);
            Response<int> res7 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res7.HasError);
            int u2 = res7.Value;
            Response<List<string>> res8 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res8.HasError);
            Assert.That(1, Is.EqualTo(res8.Value.Count));
            Response<List<string>> res13 = bridge.ReadAlerts(user);
            Assert.IsFalse(res13.HasError);
            Assert.That(0, Is.EqualTo(res13.Value.Count));
        }
        [Test]
        public void TestComplexCloseShopDelayedAlertSuccess()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res21 = bridge.AddOwner(user, shopId, "daniel123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res22 = bridge.AddManager(user2, shopId, "newuser123");
            Assert.IsFalse(res22.HasError);
            Response<bool> res3 = bridge.Logout(user2);
            Assert.IsFalse(res3.HasError);
            Response<bool> res4 = bridge.Logout(user3);
            Assert.IsFalse(res4.HasError);
            Response<bool> res5 = bridge.Logout(newuser);
            Assert.IsFalse(res5.HasError);
            Response<int> res6 = bridge.CloseShop(user, shopId);
            Assert.IsFalse(res6.HasError);
            Response<int> res7 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res7.HasError);
            int u2 = res7.Value;
            Response<List<string>> res8 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res8.HasError);
            Assert.That(2, Is.EqualTo(res8.Value.Count));
            Response<int> res9 = bridge.Login("daniel123", "daniel123");
            Assert.IsFalse(res9.HasError);
            int u3 = res9.Value;
            Response<List<string>> res10 = bridge.ReadAlerts(u3);
            Assert.IsFalse(res10.HasError);
            Assert.That(2, Is.EqualTo(res10.Value.Count));
            Response<int> res11 = bridge.Login("newuser123", "newuser123");
            Assert.IsFalse(res11.HasError);
            int nu = res11.Value;
            Response<List<string>> res12 = bridge.ReadAlerts(nu);
            Assert.IsFalse(res12.HasError);
            Assert.That(2, Is.EqualTo(res12.Value.Count));
            Response<List<string>> res13 = bridge.ReadAlerts(user);
            Assert.IsFalse(res13.HasError);
            Assert.That(1, Is.EqualTo(res13.Value.Count));
        }
        [Test]
        public void TestComplexCloseShopDelayedAlertFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res21 = bridge.AddOwner(user, shopId, "daniel123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res22 = bridge.AddManager(user2, shopId, "newuser123");
            Assert.IsFalse(res22.HasError);
            Response<bool> res3 = bridge.Logout(user2);
            Assert.IsFalse(res3.HasError);
            Response<bool> res4 = bridge.Logout(user3);
            Assert.IsFalse(res4.HasError);
            Response<bool> res5 = bridge.Logout(newuser);
            Assert.IsFalse(res5.HasError);
            Response<int> res6 = bridge.CloseShop(user2, shopId);
            Assert.IsTrue(res6.HasError);
            Response<int> res7 = bridge.Login("dan123", "dan123");
            Assert.IsFalse(res7.HasError);
            int u2 = res7.Value;
            Response<List<string>> res8 = bridge.ReadAlerts(u2);
            Assert.IsFalse(res4.HasError);
            Assert.That(1, Is.EqualTo(res8.Value.Count));
            Response<int> res9 = bridge.Login("daniel123", "daniel123");
            Assert.IsFalse(res9.HasError);
            int u3 = res9.Value;
            Response<List<string>> res10 = bridge.ReadAlerts(u3);
            Assert.IsFalse(res10.HasError);
            Assert.That(1, Is.EqualTo(res10.Value.Count));
            Response<int> res11 = bridge.Login("newuser123", "newuser123");
            Assert.IsFalse(res11.HasError);
            int nu = res11.Value;
            Response<List<string>> res12 = bridge.ReadAlerts(nu);
            Assert.IsFalse(res12.HasError);
            Assert.That(1, Is.EqualTo(res12.Value.Count));
            Response<List<string>> res13 = bridge.ReadAlerts(user);
            Assert.IsFalse(res13.HasError);
            Assert.That(0, Is.EqualTo(res13.Value.Count));
        }
    }
}
