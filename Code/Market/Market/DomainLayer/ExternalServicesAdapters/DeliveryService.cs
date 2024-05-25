using Market.DataObject;

namespace Market.DomainLayer.ExternalServicesAdapters
{
    public interface DeliveryService
    {
        Response<string> handshake();
        Response<int> createDelivery(DeliveryDetails details);
        Response<int> cancel_supply(string id);

    }
}
