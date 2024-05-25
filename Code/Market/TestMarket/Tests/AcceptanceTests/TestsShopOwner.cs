using Market.DataObject;
using Market.DataObject.OrderRecords;
using Market.ServiceLayer;
using NUnit.Framework;
//using Market.DomainLayer.Market;
//using Market.DomainLayer.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarket.Tests
{
    internal class TestsShopOwner
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

        //tests for add product
        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        [TestCase("product3", 8, "category2", "description2")]
        public void TestAddProductToShopSuccess(string product, int price, string category, string desc)
        {
            Response<int> res1 = bridge.AddProductToShop(user, shopId, product, price, category, desc);
            Assert.IsFalse(res1.HasError);
            Response<Product> res = bridge.GetProductInfo(res1.Value);
            Assert.AreEqual(product, res.Value.Name);
            Assert.AreEqual(price, res.Value.Price);
            Assert.AreEqual(category, res.Value.Category);
            Assert.AreEqual(desc, res.Value.Description);
        }

        [Test]
        [TestCase("", 10, "category1", "description1")]
        [TestCase("product2", 10, "", "")]
        [TestCase(null, 8, "category2", "description2")]
        [TestCase(null, 8, null, "description2")]
        [TestCase("product1", 8, null, "description2")]
        public void TestAddProductToShopWithInvalidParameters(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, product, price, category, desc);
            Assert.IsTrue(res.HasError);
        }


        [Test]
        public void TestAddProductToShopWithNegativePrice()
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product1", -10, "category1", "description1");
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestAddProductToShopWithZeroPrice()
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product1", 0, "category1", "description1");
            Assert.IsTrue(res.HasError);
        }

        [Test]
        [TestCase("Product1", 10, "category1", "description1")]
        [TestCase("Product2", 10, "category1", "description1")]
        public void TestAddProductToShop2Times(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, product, price, category, desc);
            Assert.IsFalse(res.HasError);
            Response<int> res2 = bridge.AddProductToShop(user, shopId, product, price, category, desc);
            Assert.IsTrue(res2.HasError);
        }

        //tests for add to Stock
        [Test]
        public void TestAddProductToStockSuccess()
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product1", 15, "category2", "");
            Response<int> res2 = bridge.AddAmountToProductInStock(user, shopId, res.Value, 25);
            Assert.IsFalse(res2.HasError);
            Assert.That(res2.Value, Is.EqualTo(25));
        }
        [Test]
        [TestCase(0)]
        [TestCase(-5)]
        public void TestAddProductToStockFailNegativAmount(int amount)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product1", 15, "category2", "");
            Assert.IsFalse(res.HasError);
            Response<int> res2 = bridge.AddAmountToProductInStock(user, shopId, res.Value, amount);
            Assert.IsTrue(res2.HasError);
        }

        //tests for remove product
        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestRemoveFromProductShopSuccess(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, product, price, category, desc);
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.RemoveProductFromShop(user, shopId, res.Value);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
        }

        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestRemoveFromProductShopFail(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, product, price, category, desc);
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.RemoveProductFromShop(user, shopId, res.Value);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<bool> res3 = bridge.RemoveProductFromShop(user, shopId, res.Value);
            Assert.IsTrue(res3.HasError);
        }

        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestRemoveFromProductShopFailExistInStock(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, product, price, category, desc);
            Assert.IsFalse(res.HasError);
            Response<int> res2 = bridge.AddAmountToProductInStock(user, shopId, res.Value, 15);
            Assert.IsFalse(res.HasError);
            Response<bool> res3 = bridge.RemoveProductFromShop(user, shopId, res.Value);
            Assert.IsTrue(res3.HasError);
        }

        //tests for edit product
        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestEditProductSuccess(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product3", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.EditProductDetails(user, shopId, res.Value, product, price, category, desc);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<Product> prod = bridge.GetProductInfo(res.Value);
            Assert.AreEqual(product, prod.Value.Name);
            Assert.AreEqual(price, prod.Value.Price);
            Assert.AreEqual(category, prod.Value.Category);
            Assert.AreEqual(desc, prod.Value.Description);
        }

        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestEditProductSuccessWithoutDesc(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product3", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.EditProductDetails(user, shopId, res.Value, product, price, category);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<Product> prod = bridge.GetProductInfo(res.Value);
            Assert.AreEqual(product, prod.Value.Name);
            Assert.AreEqual(price, prod.Value.Price);
            Assert.AreEqual(category, prod.Value.Category);
            Assert.AreNotEqual(desc, prod.Value.Description);
        }

        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestEditProductSuccessWithoutCategory(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product3", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.EditProductDetails(user, shopId, res.Value, product, price, description:desc );
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<Product> prod = bridge.GetProductInfo(res.Value);
            Assert.AreEqual(product, prod.Value.Name);
            Assert.AreEqual(price, prod.Value.Price);
            Assert.AreNotEqual(category, prod.Value.Category);
            Assert.AreEqual(desc, prod.Value.Description);
        }

        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestEditProductSuccessWithoutPrice(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product3", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.EditProductDetails(user, shopId, res.Value, product, category: category, description: desc);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<Product> prod = bridge.GetProductInfo(res.Value);
            Assert.AreEqual(product, prod.Value.Name);
            Assert.AreNotEqual(price, prod.Value.Price);
            Assert.AreEqual(category, prod.Value.Category);
            Assert.AreEqual(desc, prod.Value.Description);
        }

        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestEditProductSuccessWithoutName(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product3", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.EditProductDetails(user, shopId, res.Value, price: price, category: category, description: desc);
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<Product> prod = bridge.GetProductInfo(res.Value);
            Assert.AreNotEqual(product, prod.Value.Name);
            Assert.AreEqual(price, prod.Value.Price);
            Assert.AreEqual(category, prod.Value.Category);
            Assert.AreEqual(desc, prod.Value.Description);
        }
        
        [Test]
        public void TestEditProductFailNameExist()
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product3", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<int> res1 = bridge.AddProductToShop(user, shopId, "product4", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<bool> res2 = bridge.EditProductDetails(user, shopId, res.Value, "product4", 15, "category2", "description2");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        [TestCase("product1", 10, "category1", "description1")]
        [TestCase("product2", 10, "category1", "")]
        public void TestEditProductSuccessExistInStock(string product, int price, string category, string desc)
        {
            Response<int> res = bridge.AddProductToShop(user, shopId, "product3", 15, "category2", "description2");
            Assert.IsFalse(res.HasError);
            Response<int> res2 = bridge.AddAmountToProductInStock(user, shopId, res.Value, 15);
            Assert.IsFalse(res.HasError);
            Response<bool> res3 = bridge.EditProductDetails(user, shopId, res.Value, product, price, category, desc);
            Assert.IsFalse(res3.HasError);
            Response<Product> prod = bridge.GetProductInfo(res.Value);
            Assert.AreEqual(product, prod.Value.Name);
            Assert.AreEqual(price, prod.Value.Price);
            Assert.AreEqual(category, prod.Value.Category);
            Assert.AreEqual(desc, prod.Value.Description);
        }

        //Tests for Add owner
        [Test]
        public void TestAddOwnerSuccess()
        {
            Response<bool> res = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
        }

        [Test]
        public void TestAddOwnerFailUserNotExist()
        {
            Response<bool> res2 = bridge.AddOwner(user, shopId, "user2");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void TestAddOwnerFailNotOwner()
        {
            Response<bool> res = bridge.AddOwner(user2, shopId, "danial123"); //eyal123 is the only owner
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestAddOwnerFailAlreadyOwner()
        {
            Response<bool> res = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
            Response<bool> res2 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsTrue(res2.HasError);        
        }
        [Test]
        public void TestAddOwnerFailShopNotExist()
        {
            Response<bool> res2 = bridge.AddOwner(user, -2 , "dan123");
            Assert.IsTrue(res2.HasError);
        }
        
        //tests for add manager
        [Test]
        public void TestAddManagerSuccess()
        {
            Response<bool> res = bridge.AddManager(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
        }

        [Test]
        public void TestAddManagerFailUserNotExist()
        {
            Response<bool> res2 = bridge.AddManager(user, shopId, "user2");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void TestAddManagerFailNotOwner()
        {
            Response<bool> res = bridge.AddManager(user2, shopId, "danial123"); //eyal123 is the only owner
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestAddManagerFailAlreadyManager()
        {
            Response<bool> res = bridge.AddManager(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
            Response<bool> res2 = bridge.AddManager(user, shopId, "dan123");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void TestAddManagerFailShopNotExist()
        {
            Response<bool> res = bridge.AddManager(user, -2, "dan123");
            Assert.IsTrue(res.HasError);
        }

        //tests for Change manager permissions
        [Test]
        public void TestChangeMangerPermissionsSuccess()
        {
            Response<bool> res = bridge.AddManager(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
            Response<string> res2 = bridge.AddPermission(user, shopId, "dan123", PermissionsEnum.Permission.CLOSE_SHOP); //add permission to close shop
            Assert.IsFalse(res2.HasError);
        }

        [Test]
        public void TestChangeMangerPermissionsFailNotOwnerOfManager()
        {
            Response<bool> res = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
            Response<bool> res2 = bridge.AddManager(user, shopId, "danial123");
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<string> res3 = bridge.AddPermission(user2, shopId, "danial123", PermissionsEnum.Permission.CLOSE_SHOP); //add permission to close shop
            Assert.IsTrue(res3.HasError);
        }

        [Test]
        public void TestChangeMangerPermissionsFailNotOwner()
        {
            Response<bool> res2 = bridge.AddManager(user, shopId, "danial123");
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<string> res3 = bridge.AddPermission(user2, shopId, "danial123", PermissionsEnum.Permission.CLOSE_SHOP); //add permission to close shop
            Assert.IsTrue(res3.HasError);
        }

        //tests for close shop
        [Test]
        public void TestCloseShopSuccess()
        {
            Response<int> res = bridge.CloseShop(user, shopId);
            Assert.IsFalse(res.HasError);
            Assert.That(res.Value, Is.EqualTo(shopId));
        }

        [Test]
        public void TestCloseShopFailOwnerNotFounder()
        {
            Response<bool> res2 = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<int> res = bridge.CloseShop(user2, shopId);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestCloseShopFailManagerNotOwner()
        {
            Response<bool> res2 = bridge.AddManager(user, shopId, "dan123"); //user2
            Assert.IsFalse(res2.HasError);
            Assert.IsTrue(res2.Value);
            Response<int> res = bridge.CloseShop(user2, shopId);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestCloseShopFailNotInShop()
        {
            Response<int> res = bridge.CloseShop(user2, shopId);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestCloseShopFailShopNotExist()
        {
            Response<int> res = bridge.CloseShop(user, -2);
            Assert.IsTrue(res.HasError);
        }

        //tests for Get employee information
        [Test]
        public void TestGetEmployeeInformationSuccess()
        {
            Response<bool> res = bridge.AddManager(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
            Response<User> res2 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsFalse(res2.HasError);
            Assert.That(res2.Value.Name, Is.EqualTo("dan123"));
        }

        [Test]
        public void TestGetEmployeeInformationFailNotInShop()
        {
            Response<User> res2 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void TestGetEmployeeInformationFailUserNotExist()
        {
            Response<User> res2 = bridge.GetEmployeeInformation(user, shopId, "user2");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void TestGetEmployeeInformationFailUserNotOwner()
        {
            Response<bool> res = bridge.AddManager(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Assert.IsTrue(res.Value);
            Response<User> res2 = bridge.GetEmployeeInformation(user2, shopId, "dan123");
            Assert.IsTrue(res2.HasError);
        }
        
        //tests for Get purchase history
        [Test]
        public void TestGetShopHistorySuccess()
        {
            Response<OrdersHistory> res = bridge.GetShopPurchaseHistory(user, shopId);
            Assert.IsFalse(res.HasError);
            
        }

        [Test]
        public void TestGetShopHistoryFailNotOwner()
        {
            Response<List<Market.DataObject.OrderRecords.PastOrder>> res = bridge.GetPurchaseHistoryOfShop(user2, shopId);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void TestGetShopHistoryFailShopNotExist()
        {
            Response<List<Market.DataObject.OrderRecords.PastOrder>> res = bridge.GetPurchaseHistoryOfShop(user2, -1);
            Assert.IsTrue(res.HasError);
        }

        [Test]
        public void closeShopNotFounder()
        {
            Response<bool> res = bridge.AddOwner(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            Response<int> res2 = bridge.CloseShop(user2, shopId2);
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void removeOwnerByPromoterSuccess()
        {
            bridge.AddOwner(user, shopId, "dan123");
            Response<bool> res = bridge.RemoveOwner(user, shopId, "dan123");
            Assert.IsFalse(res.HasError);
            //TODO: check that the owner was removed
            Response<User> res2 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsTrue(res2.HasError);
        }

        [Test]
        public void removeOwnerByPromoterFail()
        {
            bridge.AddOwner(user, shopId, "dan123");
            Response<int> r = bridge.Login("danial123", "danial123");
            int u = r.Value;
            Response<bool> res = bridge.RemoveOwner(u, shopId, "dan123");
            Assert.IsTrue(res.HasError);
            //TODO: check that the owner wasnt removed
            Response<User> res2 = bridge.GetEmployeeInformation(user, shopId, "dan123");
            Assert.IsFalse(res2.HasError);
        }
        [Test]
        public void OwnerReciveMessageOnCloseShop()
        {
            bridge.AddOwner(user, shopId, "dan123");
            bridge.CloseShop(user,shopId);
            Response<List<string>> res = bridge.ReadAlerts(user2);
            Assert.IsFalse(res.HasError);
            Assert.AreEqual(1, res.Value.Count());
        }
    }
}
