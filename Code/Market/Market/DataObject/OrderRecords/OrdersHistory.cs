using Market.DataObject;

namespace Market.DataObject.OrderRecords;

public class OrdersHistory
{
    private Repository<int, PastOrder> _ordersHistory;

    public List<PastOrder> Orders { get => _ordersHistory.Values().ToList();}

    public OrdersHistory()
    {
        _ordersHistory = new ListRepository<int, PastOrder>();
    }

    public void AddOrder(PastOrder order)
    {
        _ordersHistory[order.OrderId] = order;
    }

    public Repository<int, PastOrder> GetOrdersHistory()
    {
        return _ordersHistory;
    }

}