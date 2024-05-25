using Market.DataObject;

namespace Market.DomainLayer.Market;

public class Order
{
    public static int orderCounter = 0;
    private int _id;
    private DateTime _date;
    private bool _isSucceed;
    private double _price;
    private List<(Item, int, double)> _items;
    private PaymentDetails _paymentDetails;

    public int Id
    {
        get { return _id; }
        private set { _id = value; }
    }
    public DateTime Date
    {
        get { return _date; }
        private set { _date = value; }
    }
    
    public string UserName
    {
        get => _paymentDetails.Holder; 
    }

    public Order(PaymentDetails paymentDetails)
    {
        _isSucceed = false;
        _price = 0;
        _id = orderCounter++;
        Date = DateTime.Now;
        _items = new List<(Item, int, double)>();
        _paymentDetails = paymentDetails;
    }

    public double Price
    {
        get
        {
            _price = CalculatePrice(GetItems());
            return _price;
        }
        private set { _price = value; }
    }
    
    public void AddItem(Item item, int amount, double discount)
    {
        _items.Add((item, amount, discount));
        _price += item.Price * amount * discount;
    }
    
    private double CalculatePrice(List<(Item, int, double)> lst)
    {
        double ans = 0;
        for (int i = 0; i < lst.Count; i++)
        {
            ans += lst[i].Item1.Price * ((1 - lst[i].Item3 == 0) ? 1 : 1 - lst[i].Item3) * lst[i].Item2;
        }

        return ans;
    }

    
    public List<(Item, int, double)> GetItems()
    {
        
        return _items;
    }
    

    
}