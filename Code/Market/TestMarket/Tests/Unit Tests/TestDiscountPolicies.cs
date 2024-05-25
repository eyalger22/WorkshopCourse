using Market.DomainLayer.Market;
using Market.DomainLayer.Market.DiscountPolicy;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.BasicDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.NumericDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.RestrictionsDiscount;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Moq;

namespace TestMarket.Tests.Unit_Tests
{

    internal class TestDiscountPolicies
    {


        DiscountPolicyManager _discountPolicyManager;
        IDiscountExp _discountExp1, _discountExp2;
        Mock<ShopBasket> mock;
        Mock<Item> mockItem1, mockItem2;
        int itemId1 = 1, itemId2 = 2;
        [SetUp]
        public void setUp()
        {
            _discountPolicyManager = new DiscountPolicyManager();
            _discountExp1 = new ShopDiscount(0.3);
            mock = new Mock<ShopBasket>();
            mockItem1 = new Mock<Item>();
            mockItem2 = new Mock<Item>();
            mockItem1.Setup(mk => mk.ItemId).Returns(itemId1);
            mockItem2.Setup(mk => mk.ItemId).Returns(itemId2);
            mockItem1.Setup(mk => mk.Price).Returns(10);
            mockItem2.Setup(mk => mk.Price).Returns(20);
            List<(Item, int)> ItemsList = new List<(Item, int)>();
            ItemsList.Add((mockItem1.Object, 1));
            ItemsList.Add((mockItem2.Object, 1));
            mock.Setup(mk => mk.GetItems()).Returns(ItemsList);
            _discountExp2 = new SingleItemDiscount(0.2, itemId1);

        }

        private double CalculatePrice(List<(Item, double)> lst1, List<(Item, int)> lst2)
        {
            double ans = 0;
            for (int i = 0; i < lst1.Count; i++)
            {
                ans += lst1[i].Item1.Price * ((1 - lst1[i].Item2 == 0) ? 1 : 1 - lst1[i].Item2) * lst2[i].Item2;
            }

            return ans;
        }

        [TearDown]
        public void tearDown()
        {
        }

        [Test]
        [TestCase(0.3, 0.2, 21.0, 28.0)]
        [TestCase(0.4, 0.1, 18.0, 29.0)]
        [TestCase(0.1, 0.4, 27.0, 26.0)]
        public void TestEvaluationsOfPolicyWithDiscountWorking(double d1, double d2, double priceTest1, double priceTest2)
        {
            _discountExp1 = new ShopDiscount(d1);
            _discountExp2 = new SingleItemDiscount(d2, itemId1);
            var lst1 = _discountExp1.EvaluateDiscount(mock.Object);
            double price1 = CalculatePrice(lst1, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest1, price1);
            var lst2 = _discountExp2.EvaluateDiscount(mock.Object);
            double price2 = CalculatePrice(lst2, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest2, price2);
        }
        
        [Test]
        [TestCase(2, 1,30.0, 28.0, 2)]
        [TestCase(3, 1, 30.0, 35.0, 2)]
        [TestCase(3, 1, 30.0, 50.0, 4)]
        public void TestEvaluationOfPolicyWithDiscountNotWorking(int item1Amount, int item2Amount, double priceTest1, double priceTest2, int moreEqualsAmount)
        {
            var _discountPred1 = new ItemAmountMoreEqualsRestriction(itemId1, moreEqualsAmount);
            var _discountExp3 = new ConditionalDiscount(_discountPred1, _discountExp1);
            
            var lst1 = _discountExp3.EvaluateDiscount(mock.Object);
            double price1 = CalculatePrice(lst1, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest1, price1);
            List<(Item, int)> ItemsList = new List<(Item, int)>();
            ItemsList.Add((mockItem1.Object, item1Amount));
            ItemsList.Add((mockItem2.Object, item2Amount));
            mock.Setup(mk => mk.GetItems()).Returns(ItemsList);
            var lst2 = _discountExp3.EvaluateDiscount(mock.Object);
            double price2 = CalculatePrice(lst2, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest2, price2);
        }


        [Test]
        [TestCase(0.3, 0.2, 21.0)]
        [TestCase(0.1, 0.4, 26.0)]
        public void TestAllDiscountsCombined(double d1, double d2, double priceTest)
        {
            _discountExp1 = new ShopDiscount(d1);
            _discountExp2 = new SingleItemDiscount(d2, itemId1);
            var res1 = _discountPolicyManager.AddDiscountPolicy(",", _discountExp1);
            var res2 = _discountPolicyManager.AddDiscountPolicy(",", _discountExp2);
            Assert.IsFalse(res1.HasError, res1.ErrorMsg);
            Assert.IsFalse(res2.HasError, res2.ErrorMsg);
            var res3 = _discountPolicyManager.ApplyDiscountPolicy(res1.Value);
            var res4 = _discountPolicyManager.ApplyDiscountPolicy(res2.Value);
            Assert.IsFalse(res3.HasError, res3.ErrorMsg);
            Assert.IsFalse(res4.HasError, res4.ErrorMsg);
            var lst = _discountPolicyManager.GetDiscounts(mock.Object);
            double price = CalculatePrice(lst, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest, price, "price is not as expected " + LstToStr(lst));
        }

        [Test]
        [TestCase(1, 1, true, 21.0)]
        [TestCase(3, 1, false, 30.0)]
        [TestCase(1, 3, false, 30.0)]
        public void TestAndDiscountPolicy(int itemId, int amount, bool success, double priceTest)
        {
            var _discountPred1 = new ItemInBasketRestriction(itemId);
            var _discountPred2 = new ItemAmountMoreEqualsRestriction(itemId1, amount);
            var _discountPred3 = new AndExpDiscount(_discountPred1, _discountPred2);
            var _discountExp3 = new AndDiscount(_discountPred3, _discountExp1);
            bool isSuccess = _discountPred3.Evaluate(mock.Object);
            Assert.AreEqual(success, isSuccess);
            var lst = _discountExp3.EvaluateDiscount(mock.Object);
            double price = CalculatePrice(lst, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest, price);
        }
        
        [Test]
        [TestCase(1, 1, true, 21.0)]
        [TestCase(1, 3, true, 21.0)]
        [TestCase(3, 3, false, 30.0)]
        public void TestOrDiscountPolicy(int itemId, int amount, bool success, double priceTest)
        {
            var _discountPred1 = new ItemInBasketRestriction(itemId);
            var _discountPred2 = new ItemAmountMoreEqualsRestriction(itemId1, amount);
            var _discountPred3 = new OrExpDiscount(_discountPred1, _discountPred2);
            var _discountExp3 = new OrDiscount(_discountPred3, _discountExp1);
            bool isSuccess = _discountPred3.Evaluate(mock.Object);
            Assert.AreEqual(success, isSuccess);
            var lst = _discountExp3.EvaluateDiscount(mock.Object);
            double price = CalculatePrice(lst, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest, price);
        }
        
        [Test]
        [TestCase(1, 1, false, 30.0)]
        [TestCase(1, 3, true, 21.0)]
        [TestCase(3, 3, false, 30.0)]
        public void TestXorDiscountPolicy(int itemId, int amount, bool success, double priceTest)
        {
            var _discountPred1 = new ItemInBasketRestriction(itemId);
            var _discountPred2 = new ItemAmountMoreEqualsRestriction(itemId1, amount);
            var _discountPred3 = new XorExpDiscount(_discountPred1, _discountPred2);
            var _discountExp3 = new XorDiscount(_discountPred3, _discountExp1);
            bool isSuccess = _discountPred3.Evaluate(mock.Object);
            Assert.AreEqual(success, isSuccess);
            var lst = _discountExp3.EvaluateDiscount(mock.Object);
            double price = CalculatePrice(lst, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest, price);
        }
        
        [Test]
        [TestCase(1, 1, false, 30.0)]
        [TestCase(1, 3, true, 21.0)]
        [TestCase(3, 3, false, 30.0)]
        public void TestConditionalDiscountPolicy(int itemId, int amount, bool success, double priceTest)
        {
            var _discountPred1 = new ItemInBasketRestriction(itemId);
            var _discountPred2 = new ItemAmountMoreEqualsRestriction(itemId1, amount);
            var _discountPred3 = new XorExpDiscount(_discountPred1, _discountPred2);
            var _discountExp3 = new ConditionalDiscount(_discountPred3, _discountExp1);
            bool isSuccess = _discountPred3.Evaluate(mock.Object);
            Assert.AreEqual(success, isSuccess);
            var lst = _discountExp3.EvaluateDiscount(mock.Object);
            double price = CalculatePrice(lst, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest, price);
        }
        
        
        [Test]
        [TestCase(1, 1, 28.0)]
        [TestCase(1, 3,  19.0)]
        [TestCase(3, 3, 28.0)]
        public void TestAddingDiscountPolicy(int itemId, int amount, double priceTest)
        {
            var _discountPred1 = new ItemInBasketRestriction(itemId);
            var _discountPred2 = new ItemAmountMoreEqualsRestriction(itemId1, amount);
            var _discountPred3 = new XorExpDiscount(_discountPred1, _discountPred2);
            var _discountExp3 = new ConditionalDiscount(_discountPred3, _discountExp1);
            var lstD = new List<IDiscountExp>();
            lstD.Add(_discountExp2);
            lstD.Add(_discountExp3);
            var _discountExp4 = new AddingDiscount(lstD);
            bool isSuccess = _discountPred3.Evaluate(mock.Object);
            var lst = _discountExp4.EvaluateDiscount(mock.Object);
            double price = CalculatePrice(lst, mock.Object.GetItems().ToList());
            Assert.AreEqual(priceTest, price, LstToStr(lst));
        }


        private string LstToStr(List<(Item, double)> lst)
        {
            string ans = "";
            foreach (var l in lst)
            {
                ans += "Item: " + l.Item1.Name + " Price: " + l.Item1.Price + " Discount: " + l.Item2 + "\n";
            }

            return ans;
        }
        
        
    }
}