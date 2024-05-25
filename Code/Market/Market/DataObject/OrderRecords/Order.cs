using Market.DomainLayer.Market;

namespace Market.DataObject.OrderRecords
{
    public class Order
    {
        public int Id { get; }
        public DateTime Date { get; }
        private LinkedList<(string, Item, int, double)> _successfulPurchases;
        private LinkedList<(string, Item, int, string)> _failedPurchases;
        public double Price { get; }

        public Order(DomainLayer.Market.Order? value)
        {
            Id = value.Id;
            Date = value.Date;
            //_successfulPurchases = value._successfulPurchases;
            //_failedPurchases = value._failedPurchases;
            Price = value.Price;
        }
    }
}
