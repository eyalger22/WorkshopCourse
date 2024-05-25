namespace Market.DataObject.OrderRecords;

public class PastUserOrder
{
    private readonly List<(ItemRecord, int, double)> _items;

    public readonly int _orderId;

    public readonly string _userName;

    public readonly double _totalPrice;

    public readonly DateTime _date;

    public PastUserOrder(Market.DomainLayer.Market.Order order)
    {
        _orderId = order.Id;
        _userName = order.UserName;
        _totalPrice = order.Price;
        _date = order.Date;
        _items = order.GetItems().ConvertAll(item => (new ItemRecord(item.Item1.Name, item.Item1.Price, item.Item1.Category, item.Item1.Description, item.Item1.Rank), item.Item2, item.Item3));
    }
}