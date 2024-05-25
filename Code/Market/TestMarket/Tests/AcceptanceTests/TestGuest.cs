using Market.DataObject;
using Market.ServiceLayer;
using NUnit.Framework;

namespace TestMarket.Tests
{
    public class Tests
    {
        Proxy bridge;
        [SetUp]
        public void Setup()
        {
            bridge = new Proxy();
            bridge.r = new Real(Service.GetService());
            bridge.InitTheSystem();
            bridge.Register("eyal123", "eyal123", "eyal@gmail.com", "beer sheve");
            bridge.Register("eldad123", "eldad123", "eldad@gmail.com", "beer sheve");
        }

        [TearDown]
        public void clean()
        {
            

        }


        [Test]
        public void TestRegisterOfNewUsers()
        {
            Response<bool> res = bridge.Register("dan123", "dan123", "dan@gmail.com", "beer sheve");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
        }
        [Test]
        public void TestRegisterOfRegisterdUsers()
        {
            Response<bool> res = bridge.Register("eyal123", "eyal123", "eyal@gmail.com", "beer sheve");
            Assert.IsTrue(res.HasError);
        }
        
        [Test]
        [TestCase("", "eyal123", "eyal@gmail.com", "beer sheve")]
        [TestCase("dan123", "", "eyal@gmail.com", "beer sheve")]
        [TestCase("dan123", "dan123", "", "beer sheve")]
        [TestCase("dan123", "dan123", "eyal@gmail.com", "")]
        [TestCase(null, "eyal123", "eyal@gmail.com", "beer sheve")]
        [TestCase("dan123", null, "eyal@gmail.com", "beer sheve")]
        [TestCase("dan123", "dan123", null, "beer sheve")]
        [TestCase("dan123", "dan123", "eyal@gmail.com", null)]
        public void TestRegisterWithInvalidParameters(string username, string pass, string mail, string address)
        {
            Response<bool> res = bridge.Register(username, pass, mail, address);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        [TestCase("eyal123", "eyal123")]
        [TestCase("eldad123", "eldad123")]
        public void TestLoginOfExistingUsersWithSuccess(string username, string pass)
        {
            Response<int> res = bridge.Login(username, pass);
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value > 0);
        }

        [Test]
        [TestCase("dan123", "dan123")]
        [TestCase("danial123", "danial123")]
        public void TestLoginOfNotExistingUsers(string username, string pass)
        {
            Response<int> res = bridge.Login(username, pass);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        [TestCase("eyal123", "eyal222")]
        [TestCase("eldad123", "eldad")]
        public void TestLoginOfExistingUsersWithErongPass(string username, string pass)
        {
            Response<int> res = bridge.Login(username, pass);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestEnterExit()
        {
            Response<int> res = bridge.EnterGuest();
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value > 0);
            //Exit
            Response<int> res2 = bridge.Exit(res.Value);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value > 0);
        }
    }
}