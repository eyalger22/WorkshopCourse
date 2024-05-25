namespace Market.DomainLayer.Market.DiscountStrategies;

public abstract class IDiscountStrategy
{

    protected List<Item> _items;
    protected Shop _shop;
    protected Func<(List<(Item, int)>, string), List<(Item, double)>> discountFunc;
    protected DateTime _deadline;



    public IDiscountStrategy(List<Item> items, Shop shop, Func<(List<(Item, int)>, string), List<(Item, double)>> discountFunc,
        DateTime deadline)
    {
        _items = items;
        _shop = shop;
        this.discountFunc = discountFunc;
        _deadline = deadline;
    }


    public List<(Item, double)> whatDiscount(List<(Item, int)> items, string code)
    {
        if (DateTime.Now > _deadline)
        {
            return items.ConvertAll((item) => (item.Item1, 0.0));
        }

        return discountFunc((items, code));
    }
}