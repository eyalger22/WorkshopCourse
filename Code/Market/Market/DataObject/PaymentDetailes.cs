using Market.DomainLayer.Users;

namespace Market.DataObject
{

    public class PaymentDetails
    {
        public string CardNumber;
        public string Month;
        public string Year;
        public string Holder;
        public string Ccv;
        public string Id;

        public PaymentDetails(string cardNumber, string month, string year, string holder, string ccv, string id)
        {
            CardNumber = cardNumber;
            Month = month;
            Year = year;
            Holder = holder;
            Ccv = ccv;
            Id = id;
        }

        public override string ToString()
        {
            return "Card Number: " + CardNumber + "\n" +
                   "Month: " + Month + "\n" +
                   "Year: " + Year + "\n" +
                   "Holder: " + Holder + "\n" +
                   "Ccv: " + Ccv + "\n" +
                   "Id: " + Id + "\n";
        }
    }
}