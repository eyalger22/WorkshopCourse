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
    internal class TestEmployeeChain
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
        public void TestRemoveOwnerWhoAddedAnotherManagerSuccess()
        {
            //Response<int> res1 = bridge.Login("daniel123", "daniel123");
            //Assert.IsFalse(res1.HasError);
            //int user3 = ;
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddManager(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsTrue(res5.HasError);
        }
        [Test]
        public void TestRemoveOwnerWhoAddedAnotherManagerFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddManager(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.RemoveOwner(newuser, shopId, "dan123");
            Assert.IsTrue(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsFalse(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsFalse(res5.HasError);
        }
        [Test]
        public void TestRemoveOwnerWhoAddedAnother2ManagersSuccess()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddManager(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res21 = bridge.AddManager(user2, shopId, "newuser123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res3 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsTrue(res5.HasError);
            Response<User> res6 = bridge.GetEmployeeInformation(user, shopId, "newuser123");
            Assert.IsTrue(res6.HasError);
        }
        [Test]
        public void TestRemoveOwnerWhoAddedAnother2ManagersFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddManager(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res21 = bridge.AddManager(user2, shopId, "newuser123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res3 = bridge.RemoveOwner(newuser2, shopId, "dan123");
            Assert.IsTrue(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsFalse(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsFalse(res5.HasError);
            Response<User> res6 = bridge.GetEmployeeInformation(user, shopId, "newuser123");
            Assert.IsFalse(res6.HasError);
        }
        [Test]
        public void TestRemoveOwnerWhoAddedAnotherOwnerSuccess()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsTrue(res5.HasError);
        }
        [Test]
        public void TestRemoveOwnerWhoAddedAnotherOwnerFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.RemoveOwner(newuser, shopId, "dan123");
            Assert.IsTrue(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsFalse(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsFalse(res5.HasError);
        }
        [Test]
        public void TestRemoveOwnerWhoAddedAnotherOwnerWhoAddedAnotherManagerSuccess()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res21 = bridge.AddManager(user3, shopId, "newuser123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res3 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsTrue(res5.HasError);
            Response<User> res6 = bridge.GetEmployeeInformation(user, shopId, "newuser123");
            Assert.IsTrue(res6.HasError);
        }
        [Test]
        public void TestRemoveOwnerWhoAddedAnotherOwnerWhoAddedAnotherOwnerSuccess()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res21 = bridge.AddOwner(user3, shopId, "newuser123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res3 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsTrue(res5.HasError);
            Response<User> res6 = bridge.GetEmployeeInformation(user, shopId, "newuser123");
            Assert.IsTrue(res6.HasError);
        }
        [Test]
        public void TestRemoveOwnerComplex()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res21 = bridge.AddManager(user2, shopId, "newuser123");
            Assert.IsFalse(res21.HasError);
            Response<bool> res22 = bridge.AddManager(user3, shopId, "newuser2123");
            Assert.IsFalse(res22.HasError);
            Response<bool> res3 = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res3.HasError);
            Response<User> res4 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res4.HasError);
            Response<User> res5 = bridge.GetEmployeeInformation(user, shopId, "daniel123");
            Assert.IsTrue(res5.HasError);
            Response<User> res6 = bridge.GetEmployeeInformation(user, shopId, "newuser123");
            Assert.IsTrue(res6.HasError);
            Response<User> res7 = bridge.GetEmployeeInformation(user, shopId, "newuser2123");
            Assert.IsTrue(res7.HasError);
        }
        [Test]
        public void TestAddOwnerWhoWasEmployedByAnotherOwnerFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.AddOwner(user2, shopId, "daniel123");
            Assert.IsTrue(res3.HasError);
        }
        [Test]
        public void TestAddManagerWhoWasEmployedByAnotherOwnerFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddManager(user, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.AddManager(user2, shopId, "daniel123");
            Assert.IsTrue(res3.HasError);
        }
        [Test]
        public void TestAddOwnerWhoWasEmployedByTheSameOwnerFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsTrue(res2.HasError);
        }
        [Test]
        public void TestAddManagerWhoWasEmployedByTheSameOwnerFailure()
        {
            Response<bool> res1 = bridge.AddManager(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddManager(user, shopId, "dan123");
            Assert.IsTrue(res2.HasError);
        }
        [Test]
        public void TestAddOwnerComplexFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddOwner(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.AddOwner(user, shopId, "daniel123");
            Assert.IsTrue(res3.HasError);
        }
        [Test]
        public void TestAddManagerComplexFailure()
        {
            Response<bool> res1 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res1.HasError);
            Response<bool> res2 = bridge.AddManager(user2, shopId, "daniel123");
            Assert.IsFalse(res2.HasError);
            Response<bool> res3 = bridge.AddManager(user, shopId, "daniel123");
            Assert.IsTrue(res3.HasError);
        }
    }
}
