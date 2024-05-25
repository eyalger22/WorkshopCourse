using Market.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingCart = Market.DomainLayer.Market.ShoppingCart;
using Member = Market.DomainLayer.Users.Member;
using Market.DomainLayer.Market;
using Market.DataObject;
using Shop = Market.DomainLayer.Market.Shop;

namespace TestMarket.Tests.Unit_Tests
{
    internal class TestShoppingCart
    {
        ShoppingCart _shoppingCart;
        Member user;
        Item item;
        [SetUp]
        public void setUp()
        {
             user = new Member("username","password","eldad@gmail.com","address","0525381648",DateTime.Now);
            _shoppingCart = new ShoppingCart(user);
            item = new Item("category", "name", 1.0, "description", "shopname");
        }

        [TearDown]
        public void tearDown()
        {
            Item.initItemId();
        }

        [Test]
        public void addItemSuccess()
        {
            
            _shoppingCart.AddNewItemToCart("shopname", item, 1);
            Assert.IsTrue(_shoppingCart.isExist("shopname",item));

        }

        public void removeItemSuccess()
        {
            _shoppingCart.AddNewItemToCart("shopname", item, 1);
            _shoppingCart.RemoveItemFromCart("shopname", item,1);
            Assert.IsFalse(_shoppingCart.isExist("shopname", item));
        }
        [Test]
        public void removeItemFailDontHaveBasket()
        {
            Response<int> res =_shoppingCart.RemoveItemFromCart("shopname", item,1);
            Assert.IsTrue(res.HasError);
        }
        [Test]
        public void removeItemFailwrongItem()
        {
            _shoppingCart.AddNewItemToCart("shopname", item, 1);
            Item i = new Item("c", "n", 0.5, "d", "shopname"); 
            Response<int> res = _shoppingCart.RemoveItemFromCart("shopname", i,1);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void editCartSuccsess()
        {
            _shoppingCart.AddNewItemToCart("shopname", item, 1);
            _shoppingCart.EditItemAmount("shopname", item, 2);
            int res =_shoppingCart.getItemAmount("shopname", item);
            Assert.AreEqual(2, res);
        }


        [Test]
        public void editCartfail()
        {
            _shoppingCart.AddNewItemToCart("shopname", item, 1);
            Response<bool> res1 = _shoppingCart.EditItemAmount("other", item, 2);
            Assert.IsTrue(res1.HasError);
        }




    }
}
