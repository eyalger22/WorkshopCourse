using Market.DataObject;
using Market.DomainLayer.ExternalServicesAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TestMarket.Tests.Unit_Tests
{
    internal class TestExternalSystems //Only to debug the external system functionality, can ignore this tests
    {
        /*
        [Test]
        public void TestHandShakePayment()
        {
            ExternalService service = ExternalService.Instance;
            Response<string> res = service.PaymentService.handshake();
            Assert.IsFalse(res.HasError);
            Assert.That("OK", Is.EqualTo(res.Value));
        }
        [Test]
        public void TestHandShakeDelivery()
        {
            ExternalService service = ExternalService.Instance;
            Response<string> res = service.DeliveryService.handshake();
            Assert.IsFalse(res.HasError);
            Assert.That("OK", Is.EqualTo(res.Value));
        }
        [Test]
        public void TestMakePayment()
        {
            ExternalService service = ExternalService.Instance;
            Response<int> res = service.PaymentService.makePayment(new PaymentDetails("1234123412341234","11","2024","user","111","8"));
            //Assert.IsFalse(res.HasError);
            if (res.HasError)
                Assert.That(res.ErrorMsg, Is.EqualTo("error"));
            Assert.That(res.Value,Is.InRange(10000, 100000));
        }
        [Test]
        public void TestMakeDelivery()
        {
            ExternalService service = ExternalService.Instance;
            Response<int> res = service.DeliveryService.createDelivery(new DeliveryDetails("name", "address 2", "city", "country", "123123"));
            Assert.IsFalse(res.HasError);
            if(res.HasError)
                Assert.That(res.ErrorMsg,Is.EqualTo("error"));
            Assert.That(res.Value, Is.InRange(10000,100000));
        }
        [Test]
        public void TestCancelPayment()
        {
            ExternalService service = ExternalService.Instance;
            Response<int> res = service.PaymentService.cancel_pay("10000");
            Assert.IsFalse(res.HasError);
            if (res.HasError)
                Assert.That(res.ErrorMsg, Is.EqualTo("error"));
            Assert.That(res.Value, Is.EqualTo(1));
        }
        [Test]
        public void TestCancelDelivery()
        {
            ExternalService service = ExternalService.Instance;
            Response<int> res = service.DeliveryService.cancel_supply("10000");
            Assert.IsFalse(res.HasError);
            if (res.HasError)
                Assert.That(res.ErrorMsg, Is.EqualTo("error"));
            Assert.That(res.Value, Is.EqualTo(1));
        }
        */
    }
}
