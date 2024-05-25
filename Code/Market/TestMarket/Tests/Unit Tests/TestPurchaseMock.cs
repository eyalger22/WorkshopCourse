
using Market.DataObject;
using Market.DomainLayer;
using Market.DomainLayer.ExternalServicesAdapters;
using Moq;
using NUnit.Framework;
using Market.DomainLayer.Market;
using Market.DomainLayer.Market.DiscountPolicy;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.BasicDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.NumericDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.RestrictionsDiscount;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Users;
using Shop = Market.DomainLayer.Market.Shop;
using ShoppingCart = Market.DomainLayer.Market.ShoppingCart;
using User = Market.DomainLayer.Users.User;
using Market.ServiceLayer;

namespace TestMarket.Tests.Unit_Tests
{

    internal class TestPurchaseMock
    {
        Mock<ShoppingCart> mockCart;
        Mock<Guest> mockUser;
        Mock<Member> mockMember;
        Mock<ShopBasket> mockBasket;
        Mock<Item> mockItem1, mockItem2;
        int itemId1 = 1, itemId2 = 2;
        ExternalService es;
        Mock<PaymentService> mockPaymentService;
        Mock<DeliveryService> mockDeliveryService;
        Shop _shop;
        Market.DomainLayer.Market.Market _market;
        PaymentDetails _paymentDetailsGood, _paymentDetailsBad;
        DeliveryDetails _deliveryDetailsGood, _deliveryDetailsBad;
        string paymentId = "1";
        string deliveryId = "1";
        private string userName = "123456789", memberName = "Achiya", itemName1 = "item1", itemName2 = "item2";
        private double priceToPay = 30.0;
        private List<(Item, int)> itemsList;
        private List<ShopBasket> basketst;
        [SetUp]
        public void setUp()
        {
            Service.GetService().UserService.InitTheSystem();
            Facade.InTestMode = true;
            _paymentDetailsGood = new PaymentDetails("1234567812345678", "7", "2026", "eldad", "123", "123456789");
            _paymentDetailsBad = new PaymentDetails("12345678", "5", "2023", "eldad1", "122", "123456783");
            _deliveryDetailsGood = new DeliveryDetails("eldad", "0525381648", "address", "123456789", "123456789");
            _deliveryDetailsBad = new DeliveryDetails("eldad1", "0525381643", "addres", "123456782", "12345678e");
            es = ExternalService.Instance;
            mockUser = new Mock<Guest>(userName);
            mockMember = new Mock<Member>(memberName, "adfds", "fdfsf", "fdssdf", "0525381648", DateTime.Now);
            mockMember.Setup(mk => mk.Name).Returns(memberName);
            mockMember.Setup(mk => mk.IsMember()).Returns(true);
            mockPaymentService = new Mock<PaymentService>();
            mockPaymentService.Setup(mk => mk.makePayment(_paymentDetailsGood)).Returns(new Response<int>(1));
            mockPaymentService.Setup(mk => mk.makePayment(_paymentDetailsBad)).Returns(new Response<int>("Problem with payment", 1));
            mockPaymentService.Setup(mk => mk.cancel_pay(paymentId)).Returns(new Response<int>(1));
            mockDeliveryService = new Mock<DeliveryService>();
            mockDeliveryService.Setup(mk => mk.createDelivery(_deliveryDetailsGood)).Returns(new Response<int>(1));
            mockDeliveryService.Setup(mk => mk.createDelivery(_deliveryDetailsBad)).Returns(new Response<int>("Problem with delivery", 1));
            mockDeliveryService.Setup(mk => mk.cancel_supply(deliveryId)).Returns(new Response<int>(1));
            es.PaymentService = mockPaymentService.Object;
            es.DeliveryService = mockDeliveryService.Object;
            mockCart = new Mock<ShoppingCart>();
            mockUser.Setup(mk => mk.ShoppingCart).Returns(mockCart.Object);
            basketst = new List<ShopBasket>();
            mockBasket = new Mock<ShopBasket>();
            basketst.Add(mockBasket.Object);
            IEnumerable<ShopBasket> baskets = basketst;
            mockCart.Setup(mk => mk.GetBaskets()).Returns(baskets);
            mockItem1 = new Mock<Item>();
            mockItem2 = new Mock<Item>();
            mockItem1.Setup(mk => mk.ItemId).Returns(itemId1);
            mockItem2.Setup(mk => mk.ItemId).Returns(itemId2);
            mockItem1.Setup(mk => mk.Name).Returns(itemName1);
            mockItem2.Setup(mk => mk.Name).Returns(itemName2);
            mockItem1.Setup(mk => mk.Price).Returns(10);
            mockItem2.Setup(mk => mk.Price).Returns(20);
            mockUser.Setup(mk => mk.Name).Returns(userName);
            itemsList = new List<(Item, int)>();
            _market = new Market.DomainLayer.Market.Market();
            _shop = new Shop("shop", mockMember.Object, "address", "bank");
            mockBasket.Setup(mk => mk.User).Returns(mockUser.Object);
            mockBasket.Setup(mk => mk.UserName).Returns(userName);
            mockBasket.Setup(mk => mk.ShopName).Returns(_shop.Name);
            _shop.AddBasket(mockBasket.Object);
            _shop.Stock.AddItem("c", itemName1, 10, "cat");
            _shop.Stock.AddItem("c", itemName2, 20, "cat");
            var items = _shop.Stock.GetItems();
            
            _market.AddShop(_shop);
            foreach (var item in items)
            {
                _shop.Stock.EditStock(item, 2);
            }
            itemId1 = _shop.Stock.GetItem(itemName1).ItemId;
            itemId2 = _shop.Stock.GetItem(itemName2).ItemId;
            itemsList.Add((_shop.Stock.GetItem(itemName1), 1));
            itemsList.Add((_shop.Stock.GetItem(itemName2), 1));
            
            mockBasket.Setup(mk => mk.GetItems()).Returns(itemsList);
            mockBasket.Setup(mk => mk.IsEmpty()).Returns(false);
            mockBasket.Setup(mk => mk.Clear()).Callback(() => itemsList.Clear());
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
        
        
        private double CalculatePrice(List<(Item, int, double)> lst)
        {
            double ans = 0;
            for (int i = 0; i < lst.Count; i++)
            {
                ans += lst[i].Item1.Price * ((1 - lst[i].Item3 == 0) ? 1 : 1 - lst[i].Item3) * lst[i].Item2;
            }

            return ans;
        }


        [Test]
        public void TestPurchaseSuccessGoodPaymentDetails()
        {
            
            var res = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
            Assert.IsFalse(res.HasError, res.ErrorMsg);
        }

        [Test]
        public void TestPurchaseFailBadPaymentDetails()
        {
            var res = _market.ParchaseCart(userName, _paymentDetailsBad, _deliveryDetailsGood);
            Assert.IsTrue(res.HasError, "Purchase should fail due to bad payment details");
            Assert.IsTrue(res.ErrorMsg.Contains("payment"), "Purchase should fail due to bad payment details");
        }
        
        [Test]
        public void TestPurchaseFailBadDeliveryDetails()
        {
            var res = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsBad);
            Assert.IsTrue(res.HasError, "Purchase should fail due to bad payment details");
            Assert.IsTrue(res.ErrorMsg.Contains("delivery"), "Purchase should fail due to bad delivery details");
        }
        
        [Test]
        public void TestPurchaseFailBadPaymentDetailsAndDeliveryDetails()
        {
            var res = _market.ParchaseCart(userName, _paymentDetailsBad, _deliveryDetailsBad);
            Assert.IsTrue(res.HasError, "Purchase should fail due to bad payment details");
            Assert.IsTrue(res.ErrorMsg.Contains("payment"), "Purchase should fail due to bad payment details");
        }

        [Test]
        [TestCase(-2)]
        [TestCase(-4)]
        public void TestPurchaseWhenThereIsNotStock(int reduceQuantity)
        {
            try
            {
                _shop.Stock.EditStock(_shop.Stock.GetItem(itemId1), reduceQuantity);
                var res1 = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
                Assert.IsTrue(res1.HasError, "Purchase should fail due to shortage of stock");
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, "purchase should fail due to shortage of stock");
            }
            _shop.Stock.EditStock(_shop.Stock.GetItem(itemId1), -reduceQuantity);
            var res2 = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
            Assert.IsFalse(res2.HasError, res2.ErrorMsg);
        }
        
        [Test]
        [TestCase("c", "newItem1", 30)]
        [TestCase("c", "newItem2", 40)]
        public void TestAddingItemToStoreAndThenPurchase(string category, string name, double price)
        {
            _shop.Stock.AddItem(category, name, price, "cat");
            var res1 = _shop.Stock.GetItem(name);
            itemsList.Add((res1, 2));
            var res2 = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
            Assert.IsTrue(res2.HasError, "Purchase should fail due to shortage of stock");
            _shop.Stock.EditStock(res1, 3);
            var res3 = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
            Assert.IsFalse(res3.HasError, res3.ErrorMsg);
        }
        
        [Test]
        [TestCase(30, 40, 4, 5, 2, 3)]
        [TestCase(2, 50, 3, 3, 2, 3)]
        [TestCase(33, 11, 2, 3, 2, 3)]
        [TestCase(12, 20, 4, 3, 3, 2)]
        [TestCase(3, 2, 7, 3, 2, 2)]
        [TestCase(12, 20, 3, 3, 3, 3)]
        public void TestCalculateTheRightPriceForThePurchaseSuccess(double itemPrice3, double itemPrice4, int itemQuantity3, int itemQuantity4, int itemCartQuantity3, int itemCartQuantity4)
        {
            string itemName3 = "i1";
            string itemName4 = "i2";
            double priceTest = 30 + itemPrice3 * itemCartQuantity3 + itemPrice4 * itemCartQuantity4; 
            _shop.Stock.AddItem("c", itemName3, itemPrice3, "cat");
            _shop.Stock.AddItem("c", itemName4, itemPrice4, "cat");
            var items = _shop.Stock.GetItems();
            _shop.Stock.EditStock(_shop.Stock.GetItem(itemName3), itemQuantity3);
            _shop.Stock.EditStock(_shop.Stock.GetItem(itemName4), itemQuantity4);
            itemsList.Add((_shop.Stock.GetItem(itemName3), itemCartQuantity3));
            itemsList.Add((_shop.Stock.GetItem(itemName4), itemCartQuantity4));
            var res = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
            if (itemCartQuantity3 <= itemQuantity3 && itemCartQuantity4 <= itemQuantity4)
            {
                Assert.IsFalse(res.HasError, res.ErrorMsg);
                Assert.AreEqual(priceTest, res.Value.Price);
            }
            else
            {
                Assert.IsTrue(res.HasError, "Purchase should fail due to shortage of stock");
            }

        }
        
        
        [Test]
        [TestCase(30, 40, 4, 2, 4, 3)]
        [TestCase(2, 50, 3, 3, 5, 3)]
        [TestCase(33, 11, 2, 3, 3, 3)]
        [TestCase(12, 20, 4, 3, 3, 5)]
        [TestCase(3, 2, 7, 3, 2, 5)]
        [TestCase(12, 20, 3, 3, 4, 3)]
        public void TestCalculateTheRightPriceForThePurchaseFail(double itemPrice3, double itemPrice4, int itemQuantity3, int itemQuantity4, int itemCartQuantity3, int itemCartQuantity4)
        {
            string itemName3 = "i1";
            string itemName4 = "i2";
            double priceTest = 30 + itemPrice3 * itemCartQuantity3 + itemPrice4 * itemCartQuantity4; 
            _shop.Stock.AddItem("c", itemName3, itemPrice3, "cat");
            _shop.Stock.AddItem("c", itemName4, itemPrice4, "cat");
            var items = _shop.Stock.GetItems();
            _shop.Stock.EditStock(_shop.Stock.GetItem(itemName3), itemQuantity3);
            _shop.Stock.EditStock(_shop.Stock.GetItem(itemName4), itemQuantity4);
            itemsList.Add((_shop.Stock.GetItem(itemName3), itemCartQuantity3));
            itemsList.Add((_shop.Stock.GetItem(itemName4), itemCartQuantity4));
            var res = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
            if (itemCartQuantity3 <= itemQuantity3 && itemCartQuantity4 <= itemQuantity4)
            {
                Assert.IsFalse(res.HasError, res.ErrorMsg);
                Assert.AreEqual(priceTest, res.Value.Price);
            }
            else
            {
                Assert.IsTrue(res.HasError, "Purchase should fail due to shortage of stock");
            }

        }

        [Test]
        public void TestPurchaseWithMultipleShopsSuccess()
        {
            string itemNew1 = "i1", itemNew2 = "i2";
            var _shop2 = new Shop("shop1", mockMember.Object, "address1", "bank");
            var mockBasket2 = new Mock<ShopBasket>();
            basketst.Add(mockBasket2.Object);
            mockBasket2.Setup(mk => mk.User).Returns(mockUser.Object);
            mockBasket2.Setup(mk => mk.UserName).Returns(userName);
            mockBasket2.Setup(mk => mk.ShopName).Returns(_shop2.Name);
            _shop2.AddBasket(mockBasket2.Object);
            _shop2.Stock.AddItem("c", itemNew1, 10, "cat");
            _shop2.Stock.AddItem("c", itemNew2, 20, "cat");
            var items = _shop2.Stock.GetItems();
            
            _market.AddShop(_shop2);
            foreach (var item in items)
            {
                _shop2.Stock.EditStock(item, 2);
            }
            var itemNId1 = _shop2.Stock.GetItem(itemNew1).ItemId;
            var itemNId2 = _shop2.Stock.GetItem(itemNew2).ItemId;
            var itemsList2 = new List<(Item, int)>();
            itemsList2.Add((_shop2.Stock.GetItem(itemNew1), 1));
            itemsList2.Add((_shop2.Stock.GetItem(itemNew2), 1));
            
            mockBasket2.Setup(mk => mk.GetItems()).Returns(itemsList2);
            mockBasket2.Setup(mk => mk.IsEmpty()).Returns(false);
            mockBasket2.Setup(mk => mk.Clear()).Callback(() => itemsList2.Clear());
            
            var res = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsGood);
            Assert.IsFalse(res.HasError, res.ErrorMsg);
        }
        
        [Test]
        public void TestPurchaseWithMultipleShopsFailDueToBadPaymentDetails()
        {
            string itemNew1 = "i1", itemNew2 = "i2";
            var _shop2 = new Shop("shop1", mockMember.Object, "address1", "bank");
            var mockBasket2 = new Mock<ShopBasket>();
            basketst.Add(mockBasket2.Object);
            mockBasket2.Setup(mk => mk.User).Returns(mockUser.Object);
            mockBasket2.Setup(mk => mk.UserName).Returns(userName);
            mockBasket2.Setup(mk => mk.ShopName).Returns(_shop2.Name);
            _shop2.AddBasket(mockBasket2.Object);
            _shop2.Stock.AddItem("c", itemNew1, 10, "cat");
            _shop2.Stock.AddItem("c", itemNew2, 20, "cat");
            var items = _shop2.Stock.GetItems();
            
            _market.AddShop(_shop2);
            foreach (var item in items)
            {
                _shop2.Stock.EditStock(item, 2);
            }
            var itemNId1 = _shop2.Stock.GetItem(itemNew1).ItemId;
            var itemNId2 = _shop2.Stock.GetItem(itemNew2).ItemId;
            var itemsList2 = new List<(Item, int)>();
            itemsList2.Add((_shop2.Stock.GetItem(itemNew1), 1));
            itemsList2.Add((_shop2.Stock.GetItem(itemNew2), 1));
            
            mockBasket2.Setup(mk => mk.GetItems()).Returns(itemsList2);
            mockBasket2.Setup(mk => mk.IsEmpty()).Returns(false);
            mockBasket2.Setup(mk => mk.Clear()).Callback(() => itemsList2.Clear());
            
            var res = _market.ParchaseCart(userName, _paymentDetailsBad, _deliveryDetailsGood);
            Assert.IsTrue(res.HasError, "purchase should fail due to bad payment details");
            Assert.IsTrue(res.ErrorMsg.Contains("payment"), "purchase should fail due to bad payment details");
        }
        
        [Test]
        public void TestPurchaseWithMultipleShopsFailDueToBadDeliveryDetails()
        {
            string itemNew1 = "i1", itemNew2 = "i2";
            var _shop2 = new Shop("shop1", mockMember.Object, "address1", "bank");
            var mockBasket2 = new Mock<ShopBasket>();
            basketst.Add(mockBasket2.Object);
            mockBasket2.Setup(mk => mk.User).Returns(mockUser.Object);
            mockBasket2.Setup(mk => mk.UserName).Returns(userName);
            mockBasket2.Setup(mk => mk.ShopName).Returns(_shop2.Name);
            _shop2.AddBasket(mockBasket2.Object);
            _shop2.Stock.AddItem("c", itemNew1, 10, "cat");
            _shop2.Stock.AddItem("c", itemNew2, 20, "cat");
            var items = _shop2.Stock.GetItems();
            
            _market.AddShop(_shop2);
            foreach (var item in items)
            {
                _shop2.Stock.EditStock(item, 2);
            }
            var itemNId1 = _shop2.Stock.GetItem(itemNew1).ItemId;
            var itemNId2 = _shop2.Stock.GetItem(itemNew2).ItemId;
            var itemsList2 = new List<(Item, int)>();
            itemsList2.Add((_shop2.Stock.GetItem(itemNew1), 1));
            itemsList2.Add((_shop2.Stock.GetItem(itemNew2), 1));
            
            mockBasket2.Setup(mk => mk.GetItems()).Returns(itemsList2);
            mockBasket2.Setup(mk => mk.IsEmpty()).Returns(false);
            mockBasket2.Setup(mk => mk.Clear()).Callback(() => itemsList2.Clear());
            
            var res = _market.ParchaseCart(userName, _paymentDetailsGood, _deliveryDetailsBad);
            Assert.IsTrue(res.HasError, "purchase should fail due to bad delivery details");
            Assert.IsTrue(res.ErrorMsg.Contains("delivery"), "purchase should fail due to bad delivery details");
        }

      /*  [Test]
        [TestCase()]
        public void TestPurchaseWithRegularDiscountPolicy(double discount)
        {
            
        }*/
        
        



        [TearDown]
        public void tearDown()
        {
        }
    }

}