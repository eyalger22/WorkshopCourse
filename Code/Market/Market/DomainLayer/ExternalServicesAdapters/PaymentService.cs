using Market.DataObject;

namespace Market.DomainLayer.ExternalServicesAdapters
{
    public interface PaymentService
    {
        Response<string> handshake();
        Response<int> makePayment(PaymentDetails details/*string month, string year, string holder, string cvv, string id*/);
        //Response<int> pay(string card_number, string month, string year, string holder, string cvv, string id);
        Response<int> cancel_pay(string id);
    }
}
