
using System.Data;
using Market.DomainLayer.Market;
using Market.DomainLayer.Market.DiscountPolicy;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.BasicDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.NumericDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.RestrictionsDiscount;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Market.SalePolicy;
using Market.DomainLayer.Market.SalePolicy.LogicalOperators;
using Market.DomainLayer.Market.SalePolicy.PredicatePolicies;
using Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using Market.DomainLayer.Users;
using Moq;
namespace TestMarket.Tests.Unit_Tests
{

    public class TestSalePolicies
    {
        SalePolicyManager _salePolicyManager;
        ISalePolicy _saleExp1, _saleExp2, _saleExp3;
        Mock<ShopBasket> mock;
        Mock<Item> mockItem1, mockItem2;
        Mock<Guest> mockUser;
        private string userName = "123456789";
        int itemId1 = 1, itemId2 = 2;
        private List<(Item, int)> ItemsList;

        [SetUp]
        public void setUp()
        {
            mockUser = new Mock<Guest>(userName);
            mockUser.Setup(mk => mk.Age()).Returns(20);
            _salePolicyManager = new SalePolicyManager();
            mock = new Mock<ShopBasket>();
            mockItem1 = new Mock<Item>();
            mockItem2 = new Mock<Item>();
            mockItem1.Setup(mk => mk.ItemId).Returns(itemId1);
            mockItem2.Setup(mk => mk.ItemId).Returns(itemId2);
            mockItem1.Setup(mk => mk.Name).Returns("Alcohol");
            mockItem2.Setup(mk => mk.Name).Returns("Milk");
            mockItem1.Setup(mk => mk.Price).Returns(10);
            mockItem2.Setup(mk => mk.Price).Returns(20);
            _saleExp1 = new LowerAgeRestriction(18);
            _saleExp2 = new CheckItemInBasket(item => item.ItemId == itemId1);
            _saleExp3 = new ImpliesExp(_saleExp2, _saleExp1);
            ItemsList = new List<(Item, int)>();
            ItemsList.Add((mockItem1.Object, 1));
            ItemsList.Add((mockItem2.Object, 1));
            mock.Setup(mk => mk.GetItems()).Returns(ItemsList);
            
            mock.Setup(mk => mk.User).Returns(mockUser.Object);

        }

        [Test]
        [TestCase(19)]
        [TestCase(18)]
        [TestCase(61)]
        [TestCase(37)]
        public void TestSaleLowerAgeSuccess(int age)
        {
            mockUser.Setup(mk => mk.Age()).Returns(age);
            var res = _saleExp1.CheckPolicy(mock.Object);
            Assert.IsTrue(res, "Policy should be true");
        }
        [Test]
        [TestCase(4)]
        [TestCase(17)]
        [TestCase(2)]
        [TestCase(13)]
        public void TestSaleLowerAgeFail(int age)
        {
            mockUser.Setup(mk => mk.Age()).Returns(age);
            var res = _saleExp1.CheckPolicy(mock.Object);
            Assert.IsFalse(res, "Policy should be false");
        }
        
        
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void TestItemInBasketSuccess(int itemId)
        {
            itemId = itemId == 1 ? itemId1 : itemId == 2 ? itemId2 : -1;
            _saleExp2 = new CheckItemInBasket(item => item.ItemId == itemId);
            var res = _saleExp2.CheckPolicy(mock.Object);
            Assert.IsTrue(res, "Policy should be true");
        }
        
        [Test]
        [TestCase(3)]
        [TestCase(4)]
        public void TestItemInBasketFail(int itemId)
        {
            itemId = itemId == 1 ? itemId1 : itemId == 2 ? itemId2 : -1;
            _saleExp2 = new CheckItemInBasket(item => item.ItemId == itemId);
            var res = _saleExp2.CheckPolicy(mock.Object);
            Assert.IsFalse(res, "Policy should be false");
        }
        
        
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void TestImpliesExpSuccess(int itemId)
        {
            if (itemId == 1)
            {

                var res = _saleExp3.CheckPolicy(mock.Object);
                Assert.IsTrue(res, "Policy should be true");
            }
            else
            {
                ItemsList.Remove((mockItem1.Object, 1));
                mockUser.Setup(mk => mk.Age()).Returns(13);
                var res = _saleExp3.CheckPolicy(mock.Object);
                Assert.IsTrue(res, "Policy should be true");
            }
        }
        
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void TestImpliesExpFail(int itemId)
        {
            if (itemId == 1)
            {
                mockUser.Setup(mk => mk.Age()).Returns(13);
                var res = _saleExp3.CheckPolicy(mock.Object);
                Assert.IsFalse(res, "Policy should be false");
            }
            else
            {
                ItemsList.Remove((mockItem2.Object, 1));
                mockUser.Setup(mk => mk.Age()).Returns(13);
                var res = _saleExp3.CheckPolicy(mock.Object);
                Assert.IsFalse(res, "Policy should be false");
            }
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public void TestItemAmountLessSuccess(int amount)
        {
            _saleExp1 = new CheckItemAmountLessPredicate(item => item.ItemId == itemId1,amount);
            var res = _saleExp1.CheckPolicy(mock.Object);
            Assert.IsTrue(res, "Policy should be true");
        }
        
        
        [Test]
        [TestCase(1)]
        [TestCase(0)]
        public void TestItemAmountLessFail(int amount)
        {
            _saleExp1 = new CheckItemAmountLessPredicate(item => true,amount);
            var res = _saleExp1.CheckPolicy(mock.Object);
            Assert.IsFalse(res, "Policy should be false");
        }

        [Test]
        [TestCase(2022)]
        [TestCase(2021)]
        public void TestDateSaleRestrictionSuccess(int year)
        {
            DateTime now = DateTime.Now;
            _saleExp1 = new DateSaleRestrictions(year, now.Month, now.Day);
            var res = _saleExp1.CheckPolicy(mock.Object);
            Assert.IsTrue(res, "Policy should be true");
        }
        
        [Test]
        [TestCase(-1)]
        [TestCase(-11)]
        public void TestDateSaleRestrictionFail(int year)
        {
            if (year == -11)
            {
                year = DateTime.Now.Year;
            }
            DateTime now = DateTime.Now;
            _saleExp1 = new DateSaleRestrictions(year, now.Month, now.Day);
            var res = _saleExp1.CheckPolicy(mock.Object);
            Assert.IsFalse(res, "Policy should be false");
        }
        
        
    }

}