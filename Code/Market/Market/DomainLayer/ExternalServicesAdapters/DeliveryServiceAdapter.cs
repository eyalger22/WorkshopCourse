using Market.DataObject;

namespace Market.DomainLayer.ExternalServicesAdapters
{
    public class DeliveryServiceAdapter : DeliveryService
    {
        private readonly DeliveryServiceProxy _proxy;
        public DeliveryServiceAdapter(DeliveryServiceProxy proxy)
        {
            _proxy = proxy;
        }
        public Response<int> cancel_supply(string id)
        {
            Response<int> response = _proxy.cancel_supply(id);
            if(!response.HasError && response.Value == -1)
            {
                return new Response<int>("Rejected by delivery service", 1);
            }
            return response;
        }

        public Response<int> createDelivery(DeliveryDetails details)
        {
            Response<int> response = _proxy.createDelivery(details);
            if (!response.HasError && response.Value == -1)
            {
                return new Response<int>("Rejected by delivery service", 1);
            }
            return response; ;
        }

        public Response<string> handshake()
        {
            return _proxy.handshake();
        }
    }
}
