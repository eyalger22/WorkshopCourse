using Market.DomainLayer.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stock = Market.DomainLayer.Market.Stock;
using Item = Market.DomainLayer.Market.Item;

namespace TestMarket.Tests.Unit_Tests
{
    internal class TestStock
    {

        Stock _stock;
        string category = "drink";
        string name = "coke";
        double price = 7.5;
        string description = "empty";


        [SetUp]
        public void setUp()
        {
            _stock = new Stock("shopname");
        }

        [TearDown]
        public void tearDown()
        {
            Item.initItemId();
            
        }

        [Test]
        public void testAddItemSuccsess() {
            try
            {
                _stock.AddItem(category, name, price, description);
                Item i = _stock.GetItem(name);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void testAddItemFailAlreadyExist()
        {
            try
            {
                _stock.AddItem(category, name, price, description);
                _stock.AddItem(category, name, price, description);
            }
            catch (Exception e)
            {
                return;
            }
            Assert.Fail();

        }

        [Test]
        public void testRemoveItemSuccsess()
        {
            try
            {
                _stock.AddItem(category, name, price, description);
                _stock.RemoveItem(name);
            }
            catch(Exception e)
            {
                Assert.Fail();
            }
            try
            {
                Item item = _stock.GetItem(name);
                Assert.AreEqual(null, item, "item not removed");
            }
            catch(Exception e)
            {
                return;
            }

        }

        [Test]
        public void testRemoveItemFail() {
            try
            {
                _stock.AddItem(category, name, price, description);
                _stock.RemoveItem("not the same");
            }
            catch (Exception e)
            {
                return;
            }
            Assert.Fail();
        }
        [Test]
        public void testEditStock()
        {
            Item i = null;
            try
            {
                _stock.AddItem(category, name, price, description);
                i = _stock.GetItem(name);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
            try
            {
                _stock.EditStock(i, 5);
                Assert.AreEqual(5,_stock.GetItemQuantity(i.ItemId));
            }
            catch(Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void testEditStockSucces()
        {
            Item i = null;
            Item i1 = new Item("cat1", "item1", 1.0, " ", "shopname");
            try
            {
                _stock.AddItem(category, name, price, description);
                i = _stock.GetItem(name);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
            try
            {
                _stock.EditStock(i1, 5);
                
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}