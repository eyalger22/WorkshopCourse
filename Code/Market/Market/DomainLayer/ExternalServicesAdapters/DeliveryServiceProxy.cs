using Market.DataObject;

namespace Market.DomainLayer.ExternalServicesAdapters
{
    public class DeliveryServiceProxy : DeliveryService
    {
        private readonly DeliveryServiceReal _real;
        public DeliveryServiceProxy(DeliveryServiceReal? real = null)
        {
            _real = real;
        }

        public Response<int> cancel_supply(string id)
        {
            if(_real is not null)
            {
                return _real.cancel_supply(id);
            }
            return new Response<int>("real delivery service not set", 1);
        }

        public Response<int> createDelivery(DeliveryDetails details)
        {
            if (_real is not null)
            {
                return _real.createDelivery(details);
            }
            return new Response<int>("real delivery service not set", 1);
        }

        public Response<string> handshake()
        {
            if (_real is not null)
            {
                return _real.handshake();
            }
            return new Response<string>("real delivery service not set", 1);
        }
    }
}
