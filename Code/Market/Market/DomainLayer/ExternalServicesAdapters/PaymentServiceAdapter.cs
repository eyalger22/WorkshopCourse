using Market.DataObject;

namespace Market.DomainLayer.ExternalServicesAdapters
{
    public class PaymentServiceAdapter : PaymentService
    {
        private readonly PaymentServiceProxy _proxy;
        public PaymentServiceAdapter(PaymentServiceProxy proxy)
        {
            _proxy = proxy;
        }
        public Response<int> cancel_pay(string id)
        {
            Response<int> response = _proxy.cancel_pay(id);
            if(!response.HasError && response.Value == -1)
            {
                return new Response<int>("Rejected by payment service", 1);
            }
            return response;
        }

        public Response<string> handshake()
        {
            return _proxy.handshake();
        }

        public Response<int> makePayment(PaymentDetails details)
        {
            Response<int> response = _proxy.makePayment(details);
            if (!response.HasError && response.Value == -1)
            {
                return new Response<int>("Rejected by payment service", 1);
            }
            return response;
        }
    }
}
