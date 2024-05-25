namespace Market.DomainLayer.Market.DiscountStrategies;

public class RegularDiscountStrategy : IDiscountStrategy
{
    
    public RegularDiscountStrategy(List<Item> items, Shop shop, DateTime deadline, double discount) : base(items, shop,
        (itemsQuantity) =>
        {
            return itemsQuantity.Item1.ConvertAll((item) => (item.Item1, (item.Item2 > 0 && items.Contains(item.Item1)) ? discount : 0));
        }, deadline)
    {
        
    }
}