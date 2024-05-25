namespace Market.DomainLayer.Market.DiscountStrategies;

public class HiddenDiscountStrategy : IDiscountStrategy
{
    protected string userCode; 
    public HiddenDiscountStrategy(List<Item> items, Shop shop, string code, DateTime deadline, double discount) : base(items, shop,
        itemsQuantity =>
        {
            return itemsQuantity.Item1.ConvertAll(item =>
                (item.Item1, (item.Item2 > 0 && items.Contains(item.Item1) && code == itemsQuantity.Item2) ? discount : 0));
        }, deadline)
    {
    }
}