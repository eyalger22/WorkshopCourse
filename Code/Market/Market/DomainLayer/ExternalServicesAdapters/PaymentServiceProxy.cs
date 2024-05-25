using Market.DataObject;

namespace Market.DomainLayer.ExternalServicesAdapters
{
    public class PaymentServiceProxy : PaymentService
    {
        private readonly PaymentServiceReal _real;
        public PaymentServiceProxy(PaymentServiceReal? real = null) {
            _real = real;
        }
        public Response<int> cancel_pay(string id)
        {
            if(_real is not null)
            {
                return _real.cancel_pay(id);
            }
            return new Response<int>("real payment service not set",1);
        }

        public Response<string> handshake()
        {
            if (_real is not null)
            {
                return _real.handshake();
            }
            return new Response<string>("real payment service not set", 1);
        }

        public Response<int> makePayment(PaymentDetails details)
        {
            if (_real is not null)
            {
                return _real.makePayment(details);
            }
            return new Response<int>("real payment service not set", 1);
        }
    }
}
